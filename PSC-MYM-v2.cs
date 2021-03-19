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
	public class RenkoStrategyDowJones : Strategy {
		private SMA SMA1;

		protected override void OnStateChange() {
			if (State == State.SetDefaults) {
				Description = @"PS HFT Robot.";
				Name = "RenkoStrategyDowJones v2";
				Calculate = Calculate.OnBarClose;
                /* Establece la forma en que se manejarán las órdenes de entrada
                    EntryHandling.AllEntries -> procesará todos los métodos de entrada de pedidos hasta que se alcancen las entradas máximas permitidas establecidas por la propiedad EntriesPerDirection mientras está en una posición abierta.
                    
                    EntryHanling.UniqueEntries -> procesará los métodos de entrada de pedidos hasta el máximo de entradas permitidas establecidas por la propiedad EntriesPerDirection por cada entrada con un nombre exclusivo
                */
                EntriesPerDirection = 1;
				EntryHandling = EntryHandling.AllEntries;
                // Cierre de sesión automático y tiempo previo para su cierre
				IsExitOnSessionCloseStrategy = false; 
				ExitOnSessionCloseSeconds = 30;
                //
				IsFillLimitOnTouch = false; 
                // Los últimos 256 valores del objeto de la serie se almacenarán en la memoria y serán accesibles para referencia (mejora el rendimiento de la memoria)
				MaximumBarsLookBack = MaximumBarsLookBack.TwoHundredFiftySix;
                // Determina cómo se completan las órdenes de estrategia durante los estados históricos.  
				OrderFillResolution = OrderFillResolution.High;
				OrderFillResolutionType	= BarsPeriodType.Minute;
				OrderFillResolutionValue = 1;
                //
				Slippage = 0;
				// Lanzará órdenes en REAL cuando la Posición de estrategia sea FLAT y por lo tanto coincida con la Posición de cuenta (FLAT)
				StartBehavior = StartBehavior.WaitUntilFlat;
				/*
				Tiempo en vigor de una órden:
					.Day -> serán canceladas por el broker al final de la sesión de negociación
					.Gtc -> seguirá funcionando hasta que se cancele explícitamente
					.Gtd -> seguirá funcionando hasta la fecha especificada
				*/
				TimeInForce = TimeInForce.Gtc;
				//
				TraceOrders = false;
				// Comportamiento de una estrategia cuando una orden generada por la estrategia se devuelve desde el servidor del corredor en un estado "Rechazado". El comportamiento predeterminado es detener la estrategia, cancelar cualquier orden de trabajo restante y luego cerrar cualquier posición abierta administrada por la estrategia enviando una orden de "Cerrar" para cada posición única.
				RealtimeErrorHandling = RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling = StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade = 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
			}
			else if (State == State.Configure) {
				AddDataSeries(new BarsPeriod() {
					BarsPeriodType = (BarsPeriodType)12345, //ninzaRenko's ID
					Value = 12, // Brick Size
					Value2 = 4 // Trend Threshold
				});
			}
			else if (State == State.DataLoaded) {				
				SMA1 = SMA(Close, 10); // Esta media nos reporta más beneficios operando 24/7
				//SetProfitTarget("Largo", CalculationMode.Ticks, 10);  RATIO NEGATIVO - REALIZA OPERACIONES SIN CRUZAR LA MEDIA
				//SetProfitTarget("Corto", CalculationMode.Ticks, 10);  RATIO NEGATIVO - REALIZA OPERACIONES SIN CRUZAR LA MEDIA
			}
		}

		protected override void OnBarUpdate() {
			if (BarsInProgress != 0) {
				return;	
			}

			if (CurrentBars[0] < 1) {
				return;	
			}
			/*
			//SET ENTRADA EN LARGO
			if ((Open[0] > SMA1[0]) && (Close[0] > SMA1[0]) && (Low[0] > SMA1[0])) {
				EnterLong(Convert.ToInt32(DefaultQuantity), "Largo");
			}
			
			//SET ENTRADA EN CORTO
			if ((Open[0] < SMA1[0]) && (Close[0] < SMA1[0]) && (High[0] < SMA1[0])) {
				EnterShort(Convert.ToInt32(DefaultQuantity), "Corto");
			}
			*/
			//SET ENTRADA EN LARGO
			if ((Low[1] > SMA1[1])  && (Close[1] > Close[2])) {
				EnterLong(Convert.ToInt32(DefaultQuantity), "Largo");
				//SetTrailStop(CalculationMode.Ticks, (SMA1[1]-10));
			}
			
			//SET ENTRADA EN CORTO
			if ((High[1] < SMA1[1]) && (Close[1] < Close[2])) {
				EnterShort(Convert.ToInt32(DefaultQuantity), "Corto");
				//SetTrailStop(CalculationMode.Ticks, (SMA1[1]+10));
			}
		}
	}
}
