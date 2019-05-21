using DevExpress.DashboardCommon;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;

namespace Dashboard_SqlDataProvider {
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1() {
            InitializeComponent();
            dashboardDesigner1.CreateRibbon();

            #region #SQLDataSource
            Access97ConnectionParameters access97Params = new Access97ConnectionParameters();
            access97Params.FileName = @"Data\nwind.mdb";

            DashboardSqlDataSource sqlDataSource = 
                new DashboardSqlDataSource("SQL Data Source 1", access97Params);
            SelectQuery selectQuery = SelectQueryFluentBuilder
                .AddTable("SalesPerson")
                .SelectColumns("CategoryName", "Sales Person", "OrderDate", "Extended Price")
                .Build("Query 1");
            sqlDataSource.Queries.Add(selectQuery);
            sqlDataSource.Fill();

            dashboardDesigner1.Dashboard.DataSources.Add(sqlDataSource);
            #endregion #SQLDataSource

            InitializeDashboardItems();
        }

        void InitializeDashboardItems() {
            IDashboardDataSource sqlDataSource = dashboardDesigner1.Dashboard.DataSources[0];

            ChartDashboardItem chart = new ChartDashboardItem();
            chart.DataSource = sqlDataSource; chart.DataMember = "Query 1";
            chart.Arguments.Add(new Dimension("OrderDate", DateTimeGroupInterval.MonthYear));
            chart.Panes.Add(new ChartPane());
            SimpleSeries salesAmountSeries = new SimpleSeries(SimpleSeriesType.SplineArea);
            salesAmountSeries.Value = new Measure("Extended Price");
            chart.Panes[0].Series.Add(salesAmountSeries);

            GridDashboardItem grid = new GridDashboardItem();
            grid.DataSource = sqlDataSource; grid.DataMember = "Query 1";
            grid.Columns.Add(new GridDimensionColumn(new Dimension("Sales Person")));
            grid.Columns.Add(new GridMeasureColumn(new Measure("Extended Price")));

            dashboardDesigner1.Dashboard.Items.AddRange(chart, grid);
        }
    }
}
