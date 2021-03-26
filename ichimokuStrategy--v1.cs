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
	public class ichimokuStrategy : Strategy
	{
//		protected List<double> historicoSenkouA = new List<double>();
//		protected List<double> historicoSenkouB = new List<double>();

		protected override void OnStateChange()
		{
			Print("On State Change");
			
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "ichimokuStrategy";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 960;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.High;
				OrderFillResolutionType						= BarsPeriodType.Minute;
				OrderFillResolutionValue					= 1;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= true;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelCloseIgnoreRejects;
				StopTargetHandling							= StopTargetHandling.ByStrategyPosition;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				Tenkan					= 0;
				Kijun					= 0;
				Chikou					= 0;
				SenkouA					= 0;
				SenkouB					= 0;
			}
			else if (State == State.Configure)
			{
//				historicoSenkouA.Capacity = 26;
			}
		}

		protected override void OnBarUpdate()
		{
			Print("On Bar Update");
			OnCalculateTenkan();
			OnCalculateKijun();
			OnCalculateSenkouA();
			OnCalculateSenkouB();
			OnCalculateChikou();
			
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
				return;
			
			/********************
			
			SET ENTRADA EN LARGO
			
			********************/
			if (Close[0]>Tenkan && Tenkan>Kijun)
			{
				EnterLong(Convert.ToInt32(DefaultQuantity), "Largo");
			}
			
			if(Open[0]<Kijun && Close[0]<Kijun)
			{
				ExitLong();
			}
			
			/********************
			
			SET ENTRADA EN CORTO
			
			********************/
			if (Close[0]<Tenkan && Tenkan<Kijun)
			{
				EnterShort(Convert.ToInt32(DefaultQuantity), "Corto");
			}
			
			if(Open[0]>Kijun && Close[0]>Kijun)
			{
				ExitShort();
			}
			
//			// Set LARGO
//			if ((Tenkan > Kijun) && (Close[0] > Tenkan) && (Chikou > SenkouA) && (Chikou > SenkouB))
//			{
//				EnterLong(Convert.ToInt32(DefaultQuantity), @"Largo");
//			}
			
//			 // Set CORTO
//			if ((Tenkan < Kijun) && (Close[0] < Tenkan) && (Chikou < SenkouA) && (Chikou < SenkouB))
//			{
//				EnterShort(Convert.ToInt32(DefaultQuantity), @"Corto");
//			}
			
		}
		
//		//PARA OPERAR EN REAL-TIME
//		protected override void OnMarketData(MarketDataEventArgs marketDataUpdate)
//		{
//			Print("On Market Data");
//		}
		
		//CALCULO DE LAS MEDIAS
		protected void OnCalculateTenkan()
		{	
			// Es la suma del mayor máximo y el menor mínimo dividida entre dos del período seleccionado (por defecto son 9)
			Tenkan = Math.Round(((MAX(High, 9)[0] + MIN(Low, 9)[0]) / 2));
			Print("On calculate Tenkan: " + Tenkan);
		}
		
		protected void OnCalculateKijun()
		{
			// Es la suma del mayor máximo y el menor mínimo dividida entre dos del período seleccionado (por defecto son 26)
			Kijun = Math.Round(((MAX(High, 26)[0] + MIN(Low, 26)[0]) / 2));
			Print("On calculate Kijun: " + Kijun);
		}
		
		protected void OnCalculateChikou()
		{
			// Es la cotización actual retrasada unos períodos (por defecto 26)
			Chikou = Close[0];
			Print("On calculate Chikou: " + Chikou);
		}
		
		protected void OnCalculateSenkouA()
		{
			// Es la suma de Tenka + Kijun divido entre 2. Se proyecta por defecto 26 períodos hacia delante de la cotización actual
			SenkouA = Math.Round((Tenkan + Kijun) / 2);
//			historicoSenkouA.Add(SenkouA);
			Print("On calculate Senkou A: " + SenkouA);
//			Print("Valor de historico Senkou A: " + historicoSenkouA[25]);
		}
		
		protected void OnCalculateSenkouB()
		{
			// Es la suma del mayor máximo y el menor mínimo dividida entre dos del período seleccionado (por defecto son 52)
			SenkouB = Math.Round(((MAX(High, 52)[0] + MIN(Low, 52)[0]) / 2));
			Print("On calculate Senkou B: " + SenkouB);
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(0, double.MaxValue)]
		[Display(Name="Tenkan", Order=1, GroupName="Parameters")]
		public double Tenkan
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, double.MaxValue)]
		[Display(Name="Kijun", Order=2, GroupName="Parameters")]
		public double Kijun
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, double.MaxValue)]
		[Display(Name="Chikou", Order=3, GroupName="Parameters")]
		public double Chikou
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, double.MaxValue)]
		[Display(Name="SenkouA", Order=4, GroupName="Parameters")]
		public double SenkouA
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(0, double.MaxValue)]
		[Display(Name="SenkouB", Order=5, GroupName="Parameters")]
		public double SenkouB
		{ get; set; }
		#endregion
	}
}
