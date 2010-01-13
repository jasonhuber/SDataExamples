<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <link rel="stylesheet" type="text/css" href="ext-3.1.0/resources/css/ext-all.css" />

    <!-- GC -->
 	<!-- LIBS -->
 	<script type="text/javascript" src="ext-3.1.0/adapter/ext/ext-base.js"></script>
 	<!-- ENDLIBS -->

    <script type="text/javascript" src="ext-3.1.0/ext-all.js"></script>

   <script language="javascript">
   Ext.chart.Chart.CHART_URL = 'ext-3.1.0/resources/charts.swf';

Ext.onReady(function(){

   /* var store = new Ext.data.JsonStore({
        fields:['name', 'visits'],
        data: [
            {name:'Jul 07', visits: 245000},
            {name:'Aug 07', visits: 240000},
            {name:'Sep 07', visits: 355000},
            {name:'Oct 07', visits: 375000},
            {name:'Nov 07', visits: 490000},
            {name:'Dec 07', visits: 495000},
            {name:'Jan 08', visits: 520000},
            {name:'Feb 08', visits: 620000}
        ]
    });*/

    // extra extra simple
    new Ext.Panel({
        title: 'Products and Price',
        renderTo: 'container',
        width:500,
        height:300,
        layout:'fit',

        items: {
            xtype: 'linechart',
            store: store,
            xField: 'name',
            yField: 'price'
        }
    });
});
   </script>

</head>
<body>
    <form id="form1" runat="server">

 
<div id="container">
    
</div>
</form>
</body>
</html>


