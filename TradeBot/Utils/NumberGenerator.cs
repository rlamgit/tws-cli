﻿using System;

namespace TradeBot.Utils
{
    public static class NumberGenerator
    {
        private static readonly Random random = new Random();

        public static int NextRandomInt()
        {
            return random.Next();
        }
    }
}
