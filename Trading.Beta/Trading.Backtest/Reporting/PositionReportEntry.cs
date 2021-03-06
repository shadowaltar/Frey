﻿namespace Trading.Backtest.Reporting
{
    public class PositionReportEntry
    {
        public string SecurityCode { get; set; } 
        public string SecurityName { get; set; } 
        public int Time { get; set; } 
        public double Price { get; set; } 
        public int Quantity { get; set; }

        public override string ToString()
        {
            return string.Format("[{2}] ({0}) {1}: {3} x {4}", SecurityCode, SecurityName, Time, Price, Quantity);
        }
    }
}