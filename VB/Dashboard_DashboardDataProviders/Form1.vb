Imports DevExpress.DashboardCommon
Imports DevExpress.DataAccess.ConnectionParameters
Imports DevExpress.DataAccess.Sql

Namespace Dashboard_SqlDataProvider
	Partial Public Class Form1
		Inherits DevExpress.XtraEditors.XtraForm

		Public Sub New()
			InitializeComponent()
			dashboardDesigner1.CreateRibbon()

'			#Region "#SQLDataSource"
			Dim access97Params As New Access97ConnectionParameters()
			access97Params.FileName = "Data\nwind.mdb"

			Dim sqlDataSource As New DashboardSqlDataSource("SQL Data Source 1", access97Params)
			Dim selectQuery As SelectQuery = SelectQueryFluentBuilder.AddTable("SalesPerson").SelectColumns("CategoryName", "Sales Person", "OrderDate", "Extended Price").Build("Query 1")
			sqlDataSource.Queries.Add(selectQuery)
			sqlDataSource.Fill()

			dashboardDesigner1.Dashboard.DataSources.Add(sqlDataSource)
'			#End Region ' #SQLDataSource

			InitializeDashboardItems()
		End Sub

		Private Sub InitializeDashboardItems()
			Dim sqlDataSource As IDashboardDataSource = dashboardDesigner1.Dashboard.DataSources(0)

			Dim chart As New ChartDashboardItem()
			chart.DataSource = sqlDataSource
			chart.DataMember = "Query 1"
			chart.Arguments.Add(New Dimension("OrderDate", DateTimeGroupInterval.MonthYear))
			chart.Panes.Add(New ChartPane())
			Dim salesAmountSeries As New SimpleSeries(SimpleSeriesType.SplineArea)
			salesAmountSeries.Value = New Measure("Extended Price")
			chart.Panes(0).Series.Add(salesAmountSeries)

			Dim grid As New GridDashboardItem()
			grid.DataSource = sqlDataSource
			grid.DataMember = "Query 1"
			grid.Columns.Add(New GridDimensionColumn(New Dimension("Sales Person")))
			grid.Columns.Add(New GridMeasureColumn(New Measure("Extended Price")))

			dashboardDesigner1.Dashboard.Items.AddRange(chart, grid)
		End Sub
	End Class
End Namespace
