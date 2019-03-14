

page.APPNAME = 'sandbox';
page.siteOptions = {
    billManagerChartType: 1
};



page.startSite(() => {
    page.utilities.inlineSvgs();
    page.utilities.setUpJQSwipeHandlers();
});