using IBApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TradeBot.Events;
using TradeBot.FileIO;
using TradeBot.Generated;
using TradeBot.TwsAbstractions;
using TradeBot.Utils;

namespace TradeBot
{
    public class TradeBotClient : DebugableEWrapper, INotifyPropertyChanged
    {
        private EReaderSignal readerSignal;
        private EClientSocket client;
        private EReader reader;

        private Contract contract;
        private TaskCompletionSource<IList<PositionInfo>> allPositionRequestTCS;
        private IList<PositionInfo> allPositions;

        private int currentTickerId;
        private int nextValidOrderId;

        public TradeBotClient()
        {
            PropertyChanged += OnPropertyChanged;

            readerSignal = new EReaderMonitorSignal();
            client = new EClientSocket(this, readerSignal);
            client.AsyncEConnect = false;
            // Create a reader to consume messages from the TWS. 
            // The EReader will consume the incoming messages and put them in a queue.
            reader = new EReader(client, readerSignal);
            reader.Start();
            // Once the messages are in the queue, an additional thread is needed to fetch them.
            Thread thread = new Thread(() =>
            {
                while (client.IsConnected())
                {
                    readerSignal.waitForSignal();
                    reader.processMsgs();
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<int, int, string> ErrorOccured;
        #endregion

        #region Properties
        private bool _isConnected;
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            private set
            {
                SetPropertyAndRaiseValueChangedEvent(ref _isConnected, value);
            }
        }

        private string[] _managedAccounts;
        public string[] ManagedAccounts
        {
            get
            {
                return _managedAccounts;
            }
            private set
            {
                SetPropertyAndRaiseValueChangedEvent(ref _managedAccounts, value);
            }
        }

        private string _tradedAccount;
        public string TradedAccount
        {
            get
            {
                return _tradedAccount;
            }
            set
            {
                SetPropertyAndRaiseValueChangedEvent(ref _tradedAccount, value);
            }
        }

        private Portfolio _portfolio;
        public Portfolio Portfolio
        {
            get
            {
                return _portfolio;
            }
            private set
            {
                SetPropertyAndRaiseValueChangedEvent(ref _portfolio, value);
            }
        }

        private void UpdatePortfolio(PortfolioInfo info)
        {
            Portfolio.Update(info);
            RaisePropertyValueChangedEvent(Portfolio, nameof(Portfolio));
        }

        private string _tickerSymbol;
        public string TickerSymbol
        {
            get
            {
                return _tickerSymbol;
            }
            set
            {
                SetPropertyAndRaiseValueChangedEvent(ref _tickerSymbol, value?.Trim().ToUpper());
            }
        }

        private TickData _tickData;
        public TickData TickData
        {
            get
            {
                return _tickData;
            }
            private set
            {
                SetPropertyAndRaiseValueChangedEvent(ref _tickData, value);
            }
        }

        public double GetTick(int tickType)
        {
            return TickData?.Get(tickType) ?? -1;
        }

        private void UpdateTick(int tickType, double value)
        {
            TickData.Update(tickType, value);
            RaisePropertyValueChangedEvent(TickData, nameof(TickData));
        }

        private int _stepSize;
        public int StepSize
        {
            get
            {
                return _stepSize;
            }
            set
            {
                SetPropertyAndRaiseValueChangedEvent(ref _stepSize, Math.Abs(value));
            }
        }

        protected void RaisePropertyValueChangedEvent<T>(T value, [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyValueChangedEventArgs<T>(propertyName, value, value));
        }

        protected void SetPropertyAndRaiseValueChangedEvent<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            T oldValue = field;
            if (!Equals(oldValue, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyValueChangedEventArgs<T>(propertyName, oldValue, newValue));
            }
        }
        #endregion

        #region Event Handlers
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            switch (eventArgs.PropertyName)
            {
                case nameof(TradedAccount):
                    OnTradedAccountChanged(eventArgs);
                    break;
                case nameof(TickerSymbol):
                    OnTickerSymbolChanged(eventArgs);
                    break;
            }
        }

        private void OnTradedAccountChanged(PropertyChangedEventArgs eventArgs)
        {
            var args = eventArgs as PropertyValueChangedEventArgs<string>;
            var oldValue = args.OldValue;
            var newValue = args.NewValue;

            if (!string.IsNullOrWhiteSpace(oldValue))
            {
                client.reqAccountUpdates(false, oldValue);
            }

            if (!string.IsNullOrWhiteSpace(newValue))
            {
                Portfolio = new Portfolio();

                client.reqAccountUpdates(true, newValue);
            }
            else
            {
                Portfolio = null;
            }
        }

        private void OnTickerSymbolChanged(PropertyChangedEventArgs eventArgs)
        {
            var args = eventArgs as PropertyValueChangedEventArgs<string>;
            var oldValue = args.OldValue;
            var newValue = args.NewValue;

            if (!string.IsNullOrWhiteSpace(oldValue))
            {
                client.cancelMktData(currentTickerId);
            }

            if (!string.IsNullOrWhiteSpace(newValue))
            {
                TickData = new TickData();
                contract = ContractFactory.CreateStockContract(newValue);

                currentTickerId = NumberGenerator.RandomInt();
                client.reqMktData(currentTickerId, contract, "", false, null);
            }
            else
            {
                TickData = null;
                contract = null;
            }
        }

        private int GenerateId()
        {
            return new Random().Next();
        }
        #endregion

        #region Public methods
        public void Connect(string host, int port, int clientId)
        {
            client.eConnect(host, port, clientId);
        }

        public void Disconnect()
        {
            client.eDisconnect();
        }

        public void LoadState()
        {
            AppState state = PropertySerializer.Deserialize<AppState>(PropertyFiles.STATE_FILE);

            TickerSymbol = state.TickerSymbol;
            StepSize = state.StepSize ?? 0;
        }

        public void SaveState()
        {
            AppState state = new AppState();
            state.TickerSymbol = TickerSymbol;
            state.StepSize = StepSize;

            PropertySerializer.Serialize(state, PropertyFiles.STATE_FILE);
        }

        public Task<IList<PositionInfo>> GetAllPositionsForAllAccountsAsync()
        {
            allPositionRequestTCS = new TaskCompletionSource<IList<PositionInfo>>();
            allPositions = new List<PositionInfo>();
            client.reqPositions();
            return allPositionRequestTCS.Task;
        }

        public void PlaceBuyOrder(int totalQuantity, int tickType = TickType.ASK)
        {
            PlaceOrder(OrderActions.BUY, totalQuantity, GetTick(tickType));
        }

        public void PlaceSellOrder(int totalQuantity, int tickType = TickType.BID)
        {
            PlaceOrder(OrderActions.SELL, totalQuantity, GetTick(tickType));
        }

        public void PlaceOrder(OrderActions action, int totalQuantity, double price)
        {
            if (contract == null || price <= 0)
            {
                return;
            }

            Order order = OrderFactory.CreateLimitOrder(action, totalQuantity, price);
            order.Account = TradedAccount;
            client.placeOrder(nextValidOrderId++, contract, order);
        }
        #endregion

        #region TWS callbacks
        public override void connectAck()
        {
            IsConnected = true;
        }

        public override void connectionClosed()
        {
            IsConnected = false;
        }

        public override void managedAccounts(string accounts)
        {
            ManagedAccounts = accounts
                .Split(new string[] { "," }, StringSplitOptions.None)
                .Select(s => s.Trim())
                .ToArray();
        }

        public override void nextValidId(int nextValidOrderId)
        {
            this.nextValidOrderId = nextValidOrderId;
        }

        public override void tickPrice(int tickerId, int field, double price, int canAutoExecute)
        {
            UpdateTickData(tickerId, field, price);
        }

        public override void tickSize(int tickerId, int field, int size)
        {
            UpdateTickData(tickerId, field, size);
        }

        public override void tickGeneric(int tickerId, int field, double value)
        {
            UpdateTickData(tickerId, field, value);
        }

        private void UpdateTickData(int tickerId, int tickType, double value)
        {
            if (tickerId != currentTickerId)
            {
                return;
            }

            UpdateTick(tickType, value);
        }

        public override void updatePortfolio(Contract contract, int position, double marketPrice, double marketValue, double avgCost, double unrealisedPNL, double realisedPNL, string account)
        {
            UpdatePortfolio(new PortfolioInfo(contract, position, marketPrice, marketValue, avgCost, unrealisedPNL, realisedPNL, account));
        }

        public override void position(string account, Contract contract, int position, double avgCost)
        {
            allPositions.Add(new PositionInfo(account, contract, position, avgCost));
        }

        public override void positionEnd()
        {
            allPositionRequestTCS.SetResult(allPositions);
            allPositionRequestTCS = null;
            allPositions = null;
        }

        public override void error(Exception e)
        {
            error(e.Message);
        }

        public override void error(string str)
        {
            error(-1, -1, str);
        }

        public override void error(int id, int errorCode, string errorMsg)
        {
            ErrorOccured?.Invoke(id, errorCode, errorMsg);

            switch (errorCode)
            {
                case ErrorCodes.TICKER_NOT_FOUND:
                    TickerSymbol = null;
                    break;
            }
        }

        public override void tickString(int tickerId, int field, string value)
        {
            // no-op, this is not needed for our basic feature set
            //base.tickString(tickerId, field, value);
        }

        public override void updateAccountValue(string key, string value, string currency, string account)
        {
            // no-op, this is not needed for our basic feature set
            //base.updateAccountValue(key, value, currency, account);
        }

        public override void updateAccountTime(string timestamp)
        {
            // no-op, this is not needed for our basic feature set
            //base.updateAccountTime(timestamp);
        }

        public override void accountDownloadEnd(string account)
        {
            // no-op, this is not needed for our basic feature set
            //base.accountDownloadEnd(account);
        }
        #endregion
    }
}