var data = [];
var chart;
var stickNumber = 2000;
var itemCount = 0;
var series;

function initChart() {
    // Themes begin
    am4core.useTheme(am4themes_animated);
    // Themes end

    chart = am4core.create("chartdiv", am4charts.XYChart);
    chart.hiddenState.properties.opacity = 0;

    chart.padding(0, 0, 0, 0);

    chart.zoomOutButton.disabled = true;

    data = [];
    //var visits = 10;
    var i = 0;
    itemCount = 0;

    for (i = 0; i <= stickNumber; i++) {
       // visits -= Math.round((Math.random() < 0.5 ? 1 : -1) * Math.random() * 10);
       // data.push({ date: new Date().setSeconds(i - 30), value: visits });
        data.push({ index: this.itemCount++, value: 0 });     
    }

    chart.data = data;

    var xAxis = chart.xAxes.push(new am4charts.ValueAxis());
    xAxis.renderer.grid.template.location = 0;
    xAxis.renderer.minGridDistance = 10;
   // xAxis.dateFormats.setKey("second", "ss");
   // xAxis.periodChangeDateFormats.setKey("second", "[bold]h:mm a");
   // xAxis.periodChangeDateFormats.setKey("minute", "[bold]h:mm a");
   // xAxis.periodChangeDateFormats.setKey("hour", "[bold]h:mm a");
    xAxis.renderer.inside = true;
    xAxis.renderer.axisFills.template.disabled = true;
    xAxis.renderer.ticks.template.disabled = true;

    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
   // valueAxis.tooltip.disabled = true;
    valueAxis.interpolationDuration = 0;
    valueAxis.rangeChangeDuration = 0;
    valueAxis.renderer.inside = true;
  //  valueAxis.renderer.minLabelPosition = 0.05;
  //  valueAxis.renderer.maxLabelPosition = 0.95;
    valueAxis.renderer.axisFills.template.disabled = true;
    valueAxis.renderer.ticks.template.disabled = true;

    series = chart.series.push(new am4charts.LineSeries());
    series.dataFields.valueX = "index";
    series.dataFields.valueY = "value";
    series.interpolationDuration = 0;
    series.defaultState.transitionDuration = 0;
    series.tensionX = 2;

   // chart.events.on("datavalidated", function () {
   //     xAxis.zoom({ start: 1 / 15, end: 1.2 }, false, true);
   // });

    xAxis.interpolationDuration = 0;
    xAxis.rangeChangeDuration = 0;

    // Add vertical scrollbar with preview
    chart.scrollbarY = new am4core.Scrollbar();
    chart.scrollbarY.marginLeft = 0;

    // Add horizotal scrollbar with preview
    //var scrollbarX = new am4charts.XYChartScrollbar();
    //scrollbarX.series.push(series);
    //chart.scrollbarX = scrollbarX;

    //document.addEventListener("visibilitychange", function () {
    //    if (document.hidden) {
    //        if (interval) {
    //            clearInterval(interval);
    //        }
    //    }
    //    else {
    //        startInterval();
    //    }
    //}, false);

    // add data
    //var interval;
    //function startInterval() {
    //    interval = setInterval(function () {
    //        visits =
    //            visits + Math.round((Math.random() < 0.5 ? 1 : -1) * Math.random() * 5);
    //        var lastdataItem = series.dataItems.getIndex(series.dataItems.length - 1);
    //        chart.addData(
    //            { date: new Date(lastdataItem.dateX.getTime() + 1000), value: visits },
    //            1
    //        );
    //    }, 1000);
    //}

   // startInterval();

    // all the below is optional, makes some fancy effects
    // gradient fill of the series
    series.fillOpacity = 1;
    var gradient = new am4core.LinearGradient();
    gradient.addColor(chart.colors.getIndex(0), 0.2);
    gradient.addColor(chart.colors.getIndex(0), 0);
    series.fill = gradient;

    // this makes date axis labels to fade out
    xAxis.renderer.labels.template.adapter.add("fillOpacity", function (fillOpacity, target) {
        var dataItem = target.dataItem;
        return dataItem.position;
    });

    // need to set this, otherwise fillOpacity is not changed and not set
    xAxis.events.on("validated", function () {
        am4core.iter.each(xAxis.renderer.labels.iterator(), function (label) {
            label.fillOpacity = label.fillOpacity;
        });
    });

    // this makes date axis labels which are at equal minutes to be rotated
    //xAxis.renderer.labels.template.adapter.add("rotation", function (rotation, target) {
    //    var dataItem = target.dataItem;
    //    if (dataItem.date && dataItem.date.getTime() == am4core.time.round(new Date(dataItem.date.getTime()), "minute").getTime()) {
    //        target.verticalCenter = "middle";
    //        target.horizontalCenter = "left";
    //        return -90;
    //    }
    //    else {
    //        target.verticalCenter = "bottom";
    //        target.horizontalCenter = "middle";
    //        return 0;
    //    }
    //})

    // bullet at the front of the line
    var bullet = series.createChild(am4charts.CircleBullet);
    bullet.circle.radius = 1;
    bullet.fillOpacity = 1;
    bullet.fill = chart.colors.getIndex(0);
    bullet.isMeasured = false;

    series.events.on("validated", function () {
        if (series && series.dataItems && series.dataItems.length > 0) {
            bullet.moveTo(series.dataItems.last.point);
            bullet.validatePosition();
        }
    });
}

  /** Clearn up the previous rendered chart. */
function cleanupChart() {
    // chart.data = [];
    data = [];
    for (i = 0; i <= stickNumber; i++) {
        // visits -= Math.round((Math.random() < 0.5 ? 1 : -1) * Math.random() * 10);
        // data.push({ date: new Date().setSeconds(i - 30), value: visits });
        data.push({ index: itemCount++, value: 0 });
    }
    chart.data = data;
}

function pushDataToChart(value) {
    if (series.dataItems && series.dataItems.length > 0) {
        chart.addData({ index:itemCount++, value: value }, 1);
    } else {
        chart.addData({ index:itemCount++, value: value }, 1);
    }
}

am4core.ready(function () {
    initChart();
}); // end am4core.ready()

