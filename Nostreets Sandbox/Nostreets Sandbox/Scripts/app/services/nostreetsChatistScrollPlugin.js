
function chatistScroll(options) {

    return _chartistScoll;

    function _chartistScoll(chart) {

        var defaultOptions = {
            width: Math.round(chart.labels.length / 1.5) + "00px"
        };

        options = Chartist.extend({}, defaultOptions, options);

        var styleTag = document.createElement('style');

        var cssRules = [
            '#' + chart.container.id + ' { overflow-x: scroll; overflow-y: hidden; }',
            '#' + chart.container.id + '::-webkit-scrollbar  { width: 0px; }',
            '#' + chart.container.id + '::-webkit-scrollbar * { background: transparent; }',
            '#' + chart.container.id + '::-webkit-scrollbar-thumb { background: rgba(0, 0, 0, 0.4) !important; }',
            '#' + chart.container.id + '::-webkit-scrollbar-track {  -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,0.3); border-radius: 10px; background-color: #F5F5F5; }',
            '#' + chart.container.id + '::-webkit-scrollbar {  width: 12px; background-color: #F5F5F5; }',
            '#' + chart.container.id + '::-webkit-scrollbar-thumb {  border-radius: 14px; -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,.3); background-color: #555; }'
        ];

        for (var css of cssRules) {
            style.appendChild(document.createTextNode(css));
        }

        document.appendChild(style);

    }

};