using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CQRS2012.Gui.Models;
using CQRS2012.Gui.Models.Services;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;


namespace CQRS2012.Gui.Controllers
{
    [Authorize]
    public class ChartController : Controller
    {
        private readonly IGameService _gameService;

        public ChartController(IGameService gameService)
        {
            this._gameService = gameService;
        }

        public ActionResult Index()
        {
            var chartData = this._gameService.GetDataForChart();
            var gamesNumbers = Enumerable.Range(0, chartData.FirstOrDefault().Item2.Length);
            var categories = gamesNumbers.Select(x => x.ToString()).ToArray();
            var seriesList = chartData.Select(x => new Series { Name = x.Item1, Data = new Data(x.Item2) }).ToArray();

            var chart = new Highcharts("chart")
            .InitChart(new Chart
            {
                DefaultSeriesType = ChartTypes.Line,
                Height = 600,
                ZoomType = ZoomTypes.Xy,
                MarginRight = 130,
                MarginBottom = 55,
                ClassName = "chart"
            })
         
            .SetTitle(new Title
            {
                Text = "Punktacja",
                X = -20
            })
                  .SetSubtitle(new Subtitle
                  {
                      Text = "",
                      X = -20
                  })
            .SetXAxis(new XAxis
            {
                Categories = categories,
                Title = new XAxisTitle { Text = "Mecze" },
                ShowFirstLabel = false,
                StartOnTick = false,
                EndOnTick = true,
                Min= 0,
                ShowLastLabel = true
            })
            .SetYAxis(new YAxis
            {
                Title = new XAxisTitle { Text = "Punkty" },
                Min = 0,
                PlotLines = new[]
                                                  {
                                                      new XAxisPlotLines
                                                      {
                                                          Value = 0,
                                                          Width = 1,
                                                          Color = ColorTranslator.FromHtml("#808080")
                                                      }
                                                  }
            })
            .SetTooltip(new Tooltip
            {
                Formatter = @"function() {
                                                return '<b>'+ this.series.name +'</b><br/>'+
                                            'mecz '+this.x +': pkt. '+ this.y;
                                        }"
            })
            .SetLegend(new Legend
            {
                Layout = Layouts.Vertical,
                Align = HorizontalAligns.Right,
                VerticalAlign = VerticalAligns.Top,
                X = -10,
                Y = 100,
                BorderWidth = 0
            })
            .SetSeries(seriesList);

            return View(chart);
        }
    }
}
