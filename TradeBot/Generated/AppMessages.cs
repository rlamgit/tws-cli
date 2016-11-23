﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TradeBot.Generated
{

    public class MenuOptionEntries
    {

        [JsonProperty("setTickerSymbol")]
        public IList<string> SetTickerSymbol { get; set; }

        [JsonProperty("setSharesFromCash")]
        public IList<string> SetSharesFromCash { get; set; }

        [JsonProperty("setShares")]
        public IList<string> SetShares { get; set; }

        [JsonProperty("setSharesFromPosition")]
        public IList<string> SetSharesFromPosition { get; set; }

        [JsonProperty("buy")]
        public IList<string> Buy { get; set; }

        [JsonProperty("sell")]
        public IList<string> Sell { get; set; }

        [JsonProperty("reversePosition")]
        public IList<string> ReversePosition { get; set; }

        [JsonProperty("closePosition")]
        public IList<string> ClosePosition { get; set; }

        [JsonProperty("listPositions")]
        public IList<string> ListPositions { get; set; }

        [JsonProperty("loadState")]
        public IList<string> LoadState { get; set; }

        [JsonProperty("saveState")]
        public IList<string> SaveState { get; set; }

        [JsonProperty("clearScreen")]
        public IList<string> ClearScreen { get; set; }

        [JsonProperty("showMenu")]
        public IList<string> ShowMenu { get; set; }
    }

    public class AppMessages
    {

        [JsonProperty("appName")]
        public string AppName { get; set; }

        [JsonProperty("titleTickerSymbol")]
        public string TitleTickerSymbol { get; set; }

        [JsonProperty("titleShares")]
        public string TitleShares { get; set; }

        [JsonProperty("titlePositionSize")]
        public string TitlePositionSize { get; set; }

        [JsonProperty("titleLastFormat")]
        public string TitleLastFormat { get; set; }

        [JsonProperty("titleBidAskFormat")]
        public string TitleBidAskFormat { get; set; }

        [JsonProperty("titleVolumeFormat")]
        public string TitleVolumeFormat { get; set; }

        [JsonProperty("titleCloseFormat")]
        public string TitleCloseFormat { get; set; }

        [JsonProperty("titleOpenFormat")]
        public string TitleOpenFormat { get; set; }

        [JsonProperty("titleUnavailable")]
        public string TitleUnavailable { get; set; }

        [JsonProperty("titleDivider")]
        public string TitleDivider { get; set; }

        [JsonProperty("menuTitle")]
        public string MenuTitle { get; set; }

        [JsonProperty("menuTitleDividerChar")]
        public string MenuTitleDividerChar { get; set; }

        [JsonProperty("menuOptionDividerChar")]
        public string MenuOptionDividerChar { get; set; }

        [JsonProperty("menuEndDividerChar")]
        public string MenuEndDividerChar { get; set; }

        [JsonProperty("menuOptionFormat")]
        public string MenuOptionFormat { get; set; }

        [JsonProperty("menuOptionEntries")]
        public MenuOptionEntries MenuOptionEntries { get; set; }

        [JsonProperty("longestMenuOptionKey")]
        public string LongestMenuOptionKey { get; set; }

        [JsonProperty("invalidMenuOption")]
        public string InvalidMenuOption { get; set; }

        [JsonProperty("welcomeMessage")]
        public string WelcomeMessage { get; set; }

        [JsonProperty("twsConnected")]
        public string TwsConnected { get; set; }

        [JsonProperty("twsDisconnected")]
        public string TwsDisconnected { get; set; }

        [JsonProperty("multipleAccountsWarningFormat")]
        public string MultipleAccountsWarningFormat { get; set; }

        [JsonProperty("accountTypeLive")]
        public string AccountTypeLive { get; set; }

        [JsonProperty("accountTypePaper")]
        public string AccountTypePaper { get; set; }

        [JsonProperty("paperAccountPrefix")]
        public string PaperAccountPrefix { get; set; }

        [JsonProperty("selectTickerPrompt")]
        public string SelectTickerPrompt { get; set; }

        [JsonProperty("tickerSymbolSetFormat")]
        public string TickerSymbolSetFormat { get; set; }

        [JsonProperty("tickerSymbolClearedFormat")]
        public string TickerSymbolClearedFormat { get; set; }

        [JsonProperty("tickerSymbolNotSetError")]
        public string TickerSymbolNotSetError { get; set; }

        [JsonProperty("sharesPrompt")]
        public string SharesPrompt { get; set; }

        [JsonProperty("sharesSetFormat")]
        public string SharesSetFormat { get; set; }

        [JsonProperty("sharesNotSetError")]
        public string SharesNotSetError { get; set; }

        [JsonProperty("cashPrompt")]
        public string CashPrompt { get; set; }

        [JsonProperty("cashSetFormat")]
        public string CashSetFormat { get; set; }

        [JsonProperty("listPositionsFormat")]
        public string ListPositionsFormat { get; set; }

        [JsonProperty("positionNotFoundError")]
        public string PositionNotFoundError { get; set; }

        [JsonProperty("positionsNotFoundError")]
        public string PositionsNotFoundError { get; set; }

        [JsonProperty("loadedStateFormat")]
        public string LoadedStateFormat { get; set; }

        [JsonProperty("savedStateFormat")]
        public string SavedStateFormat { get; set; }

        [JsonProperty("commissionFormat")]
        public string CommissionFormat { get; set; }

        [JsonProperty("priceDataUnavailableError")]
        public string PriceDataUnavailableError { get; set; }

        [JsonProperty("invalidIntegerInputError")]
        public string InvalidIntegerInputError { get; set; }

        [JsonProperty("invalidDecimalInputError")]
        public string InvalidDecimalInputError { get; set; }

        [JsonProperty("invalidPositiveInputError")]
        public string InvalidPositiveInputError { get; set; }

        [JsonProperty("invalidNonEmptyStringInputError")]
        public string InvalidNonEmptyStringInputError { get; set; }

        [JsonProperty("twsErrorFormat")]
        public string TwsErrorFormat { get; set; }

        [JsonProperty("appExceptionMessageFormat")]
        public string AppExceptionMessageFormat { get; set; }

        [JsonProperty("appExceptionStackTraceFormat")]
        public string AppExceptionStackTraceFormat { get; set; }

        [JsonProperty("pressAnyKeyToExit")]
        public string PressAnyKeyToExit { get; set; }
    }

}
