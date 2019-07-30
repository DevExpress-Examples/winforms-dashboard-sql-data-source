using DevExpress.DashboardCommon;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;

namespace Dashboard_SqlDataProvider
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1() {
            InitializeComponent();
            dashboardDesigner1.CreateRibbon();

            DataConnectionParametersBase connParameters = CreateConnectionParameters("MSAccess");
            DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source 1", connParameters);
            sqlDataSource.Queries.Add(CreateQuery("fluent"));
            sqlDataSource.Fill();
            dashboardDesigner1.Dashboard = CreateDashboard(sqlDataSource);
        }

        private DataConnectionParametersBase CreateConnectionParameters(string providerName)
        {
            switch (providerName)
            {
                case "MSAccess":
                    return new Access97ConnectionParameters()
                    {
                        FileName = @"Data\nwind.mdb"
                    };
                case "MSSqlServer":
                    return new MsSqlConnectionParameters()
                    {
                        ServerName = "localhost",
                        DatabaseName = "Northwind",
                        AuthorizationType = MsSqlAuthorizationType.Windows
                    };
                default:
                    return new XmlFileConnectionParameters()
                    {
                        FileName = @"Data\sales-person.xml"
                    };
            }
        }
        private SqlQuery CreateQuery(string builderName)
        {
            switch (builderName)
            {
                case "fluent":
                    return SelectQueryFluentBuilder
                        .AddTable("SalesPersons")
                        .SelectColumns("CategoryName", "SalesPerson", "OrderDate", "ExtendedPrice")
                        .Build("Query 1");
                default:
                    return new CustomSqlQuery()
                    {
                        Name = "Query 1",
                        Sql = @"SELECT CategoryName, SalesPerson, OrderDate, ExtendedPrice FROM SalesPersons"
                    };
            }
        }

        private Dashboard CreateDashboard(IDashboardDataSource dataSource)
        {
            Dashboard newDashboard = new Dashboard();
            newDashboard.DataSources.Add(dataSource);

            ChartDashboardItem chart = new ChartDashboardItem
            {
                DataSource = dataSource,
                DataMember = "Query 1"
            };
            chart.Arguments.Add(new Dimension("OrderDate", DateTimeGroupInterval.MonthYear));
            chart.Panes.Add(new ChartPane());
            SimpleSeries salesAmountSeries = new SimpleSeries(SimpleSeriesType.SplineArea)
            {
                Value = new Measure("ExtendedPrice")
            };
            chart.Panes[0].Series.Add(salesAmountSeries);

            GridDashboardItem grid = new GridDashboardItem
            {
                DataSource = dataSource,
                DataMember = "Query 1"
            };
            grid.Columns.Add(new GridDimensionColumn(new Dimension("SalesPerson")));
            grid.Columns.Add(new GridMeasureColumn(new Measure("ExtendedPrice")));

            newDashboard.Items.AddRange(chart, grid);
            return newDashboard;
        }
    }
}
