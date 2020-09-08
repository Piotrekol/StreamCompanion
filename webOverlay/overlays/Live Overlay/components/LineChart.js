
const LineChart = {
    name: 'LineChart',
    template: `
    <div class="chart-container">
        <slot />
        <canvas ref="chart"></canvas>
    </div>
  `,
    props: {
        points: {},
        settings: {
            default: {}
        }
    },
    data: () => ({
        chart: {},
        defaultSettings: {
            maxTicks: 3,
            backgroundColor: 'rgba(255, 99, 132,0.6)',
            yAxesFontColor: 'white'
        }
    }),
    methods: {
        updateChart() {
            this.chart.data.labels = [];
            this.chart.data.datasets.forEach((dataset) => {
                dataset.data = [];
            });

            this.chart.data.labels = this.points.map(x => x[0]);
            let values = this.points.map(x => x[1])
            this.chart.data.datasets.forEach((dataset) => {
                dataset.data.push(...values);

            });

            this.chart.update();
        }
    },
    computed: {

    },
    watch: {
        points() {
            this.updateChart()
        },
        settings() {
            console.log('settings updated:', JSON.stringify(this.settings));
            let settings = { ...this.defaultSettings, ...this.settings };

            this.chart.data.datasets[0].backgroundColor = settings.backgroundColor;
            this.chart.options.scales.yAxes[0].ticks.fontColor = settings.yAxesFontColor;

            this.chart.update();
        }

    },
    mounted: function () {
        let settings = { ...this.settings, ...this.defaultSettings }
        var ctx = this.$refs.chart.getContext('2d');
        this.chart = new Chart(ctx, {
            type: 'line',

            data: {
                labels: ['0', '1', '2', '3'],
                datasets: [{
                    backgroundColor: settings.backgroundColor,
                    //borderColor: this.backgroundColor,
                    data: [0, 10, 20, 30]
                }]
            },

            // Configuration options go here
            options: {
                maintainAspectRatio: false,
                responsive: true,
                legend: {
                    display: false
                },
                scales: {
                    xAxes: [{
                        ticks: {
                            display: false
                        },
                        gridLines: {
                            zeroLineColor: 'transparent',
                            color: "transparent"
                        }
                    }],
                    yAxes: [{
                        ticks: {
                            beginAtZero: true,
                            padding: -25,
                            fontColor: settings.yAxesFontColor,
                            maxTicksLimit: settings.maxTicks,

                        },
                        gridLines: {
                            zeroLineColor: 'transparent',
                            color: "transparent"
                        }
                    }]
                },
                elements: {
                    point: {
                        radius: 0
                    }
                },
                //animation: {
                //  duration: 250
                //}
            }
        });
    }
}


export default LineChart