#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class RenkoStrategyDowJones : Strategy
	{
		private SMA SMA1;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "RenkoStrategyDowJones";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 960;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.Infinite;
				OrderFillResolution							= OrderFillResolution.High;
				OrderFillResolutionType						= BarsPeriodType.Minute;
				OrderFillResolutionValue					= 1;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 2;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
			}
			else if (State == State.Configure)
			{
				AddDataSeries(new BarsPeriod()
				{
					BarsPeriodType = (BarsPeriodType)12345,	//ninzaRenko's ID
					Value = 12,								// Brick Size
					Value2 = 4								// Trend Threshold
				});
				
			}
			else if (State == State.DataLoaded)
			{				
				SMA1 = SMA(Close, 50);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0)
			{
				return;	
			}

			if (CurrentBars[0] < 1)
			{
				return;	
			}
			
			//SET ENTRADA EN LARGO
			if ((Open[0] > SMA1[0]) && (Close[0] > SMA1[0]) && (Low[0] > SMA1[0]))
			{
				EnterLong(Convert.ToInt32(DefaultQuantity), "Largo");
			}
			
			//SET ENTRADA EN CORTO
			if ((Open[0] < SMA1[0]) && (Close[0] < SMA1[0]) && (High[0] < SMA1[0]))
			{
				EnterShort(Convert.ToInt32(DefaultQuantity), "Corto");
			}
		}
	}
}
