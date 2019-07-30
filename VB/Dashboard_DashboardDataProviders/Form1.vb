Imports DevExpress.DashboardCommon
Imports DevExpress.DataAccess.ConnectionParameters
Imports DevExpress.DataAccess.Sql

Namespace Dashboard_SqlDataProvider
	Partial Public Class Form1
		Inherits DevExpress.XtraEditors.XtraForm

		Public Sub New()
			InitializeComponent()
			dashboardDesigner1.CreateRibbon()

			Dim connParameters As DataConnectionParametersBase = CreateConnectionParameters("MSAccess")
			Dim sqlDataSource As New DashboardSqlDataSource("SQL Data Source 1", connParameters)
			sqlDataSource.Queries.Add(CreateQuery("fluent"))
			sqlDataSource.Fill()
			dashboardDesigner1.Dashboard = CreateDashboard(sqlDataSource)
		End Sub

		Private Function CreateConnectionParameters(ByVal providerName As String) As DataConnectionParametersBase
			Select Case providerName
				Case "MSAccess"
					Return New Access97ConnectionParameters() With {.FileName = "Data\nwind.mdb"}
				Case "MSSqlServer"
					Return New MsSqlConnectionParameters() With {.ServerName = "localhost", .DatabaseName = "Northwind", .AuthorizationType = MsSqlAuthorizationType.Windows}
				Case Else
					Return New XmlFileConnectionParameters() With {.FileName = "Data\sales-person.xml"}
			End Select
		End Function
		Private Function CreateQuery(ByVal builderName As String) As SqlQuery
			Select Case builderName
				Case "fluent"
					Return SelectQueryFluentBuilder.AddTable("SalesPersons").SelectColumns("CategoryName", "SalesPerson", "OrderDate", "ExtendedPrice").Build("Query 1")
				Case Else
					Return New CustomSqlQuery() With {.Name = "Query 1", .Sql = "SELECT CategoryName, SalesPerson, OrderDate, ExtendedPrice FROM SalesPersons"}
			End Select
		End Function

		Private Function CreateDashboard(ByVal dataSource As IDashboardDataSource) As Dashboard
			Dim newDashboard As New Dashboard()
			newDashboard.DataSources.Add(dataSource)

			Dim chart As ChartDashboardItem = New ChartDashboardItem With {.DataSource = dataSource, .DataMember = "Query 1"}
			chart.Arguments.Add(New Dimension("OrderDate", DateTimeGroupInterval.MonthYear))
			chart.Panes.Add(New ChartPane())
			Dim salesAmountSeries As New SimpleSeries(SimpleSeriesType.SplineArea) With {.Value = New Measure("ExtendedPrice")}
			chart.Panes(0).Series.Add(salesAmountSeries)

			Dim grid As GridDashboardItem = New GridDashboardItem With {.DataSource = dataSource, .DataMember = "Query 1"}
			grid.Columns.Add(New GridDimensionColumn(New Dimension("SalesPerson")))
			grid.Columns.Add(New GridMeasureColumn(New Measure("ExtendedPrice")))

			newDashboard.Items.AddRange(chart, grid)
			Return newDashboard
		End Function
	End Class
End Namespace
