using System.Web.Optimization;
using NostreetsExtensions.Extend.Web;

namespace Nostreets_Sandbox
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            DefineOrder(bundles);


            #region Base
            //+BOOTSTRAP CSS
            bundles.Add(
                  new StyleBundle("~/bootstrapCss").IncludeWithCssRewriteUrlTransform(

                      "~/node_modules/bootstrap/dist/css/bootstrap-theme.css",
                      "~/node_modules/bootstrap/dist/css/bootstrap.css"
                  )
              );

            //+JQUERY
            bundles.Add(new ScriptBundle("~/jquery").Include("~/node_modules/jquery/dist/jquery.js"));

            //+JQUERY REQUIREMENTS
            bundles.Add(
                new ScriptBundle("~/jqueryRequirements").Include(

                    //+VALIDATION
                    "~/node_modules/jquery-validation/dist/jquery.validate.js",

                    //+POPPER
                    "~/node_modules/popper.js/dist/umd/popper.js",

                    //+BOOTSTRAP
                    "~/node_modules/bootstrap/dist/js/bootstrap.js",


                    //+MATERIAL DESIGN LITE
                    "~/node_modules/material-design-lite/material.js",
                    "~/assets/js/material.min.js"


                )
            );

            //+ANGULAR REQUIREMENTS
            bundles.Add(
                 new ScriptBundle("~/angularRequirements").Include(

                     "~/node_modules/angular/angular.js",
                     "~/node_modules/angular-animate/angular-animate.js",
                     "~/node_modules/angular-route/angular-route.js",
                     "~/node_modules/angular-sanitize/angular-sanitize.js",
                     "~/node_modules/angular-cookies/angular-cookies.js",

                     "~/node_modules/angular-toastr/dist/angular-toastr.tpls.min.js",
                     "~/node_modules/angular-ui-bootstrap/dist/ui-bootstrap-tpls.js",

                     "~/node_modules/tinycolor2/dist/tinycolor-min.js",
                     "~/node_modules/angularjs-color-picker/dist/angularjs-color-picker.js"

                 )
             ); 
            #endregion


            #region Nostreets Portfolio
            //+Nostreets Portfolio CSS
            bundles.Add(
                  new StyleBundle("~/nostreetsCss").IncludeWithCssRewriteUrlTransform(

                      //+MATERIAL DESIGN LITE
                      "~/node_modules/material-design-lite/material.css",
                      "~/node_modules/material-design-lite/dist/material.blue_grey-indigo.min.css",

                      //+CUSTOM
                      "~/assets/css/theme-Dark.css",
                      "~/assets/css/material-dashboard.css",
                      "~/assets/css/chartist-plugin-tooltip.css",
                      "~/assets/css/nostreets.css",

                      //+OTHER
                      "~/assets/css/font-awesome.css",
                      "~/node_modules/angular-ui-bootstrap/dist/ui-bootstrap-csp.css",
                      "~/node_modules/angular-toastr/dist/angular-toastr.css",
                      "~/node_modules/angularjs-color-picker/dist/angularjs-color-picker.css",
                      "~/node_modules/angularjs-color-picker/dist/themes/angularjs-color-picker-bootstrap.min.css"
                 )
              );


            //+Nostreets Portfolio JAVASCRIPT
            bundles.Add(
                new ScriptBundle("~/nostreetsJS").Include(

                    //+OTHER
                    "~/assets/js/sweetalert2.js",
                    "~/assets/js/chartist.min.js",                                      //Chartist.js
                    "~/assets/js/chartist-plugin-tooltip.min.js",
                    "~/assets/js/chartist-plugin-zoom.min.js",

                    //+CUSTOM
                    "~/assets/js/particles.js",
                    "~/assets/js/renderNostreetsParticles.js",                                   // Background Particles
                    "~/Scripts/shared/services/nostreetsChartistScrollPlugin.js",       // Nostreets Chartist Scroll Plugin
                    "~/Scripts/shared/core/page.js",
                    "~/Scripts/nostreets/app/page.js",
                    "~/Scripts/shared/core/extend.js",
                    "~/Scripts/shared/services/alertService.js",
                    "~/Scripts/shared/services/systemEvent.service.js",
                    "~/Scripts/shared/core/baseController.js"
                )
            );


            //+Nostreets Portfolio ANGULAR
            bundles.Add(
                new ScriptBundle("~/nostreetsAngular").Include(
                    "~/Scripts/nostreets/app/configs/routeConfig.js",
                    "~/Scripts/shared/services/apiService.js"
                )
            );
            #endregion


            OBL_Website.BundleConfig.RegisterOBLBundles(bundles);


            //+THEME SCRIPTS
            bundles.Add(
                new ScriptBundle("~/themeScripts").Include(
                    "~/assets/js/animsition.min.js",
                    "~/assets/js/jquery.magnific-popup.min.js",
                    "~/assets/js/jquery.countdown.min.js",
                    "~/assets/js/twitterFetcher_min.js",
                    "~/assets/js/masonry.pkgd.min.js",
                    "~/assets/js/imagesloaded.pkgd.min.js",
                    "~/assets/js/jquery.flexslider-min.js",
                    "~/assets/js/photoswipe.min.js",
                    "~/assets/js/photoswipe-ui-default.min.js",
                    "~/assets/js/jqinstapics.min.js",
                    "~/assets/js/startUpPageScript.js"
                )
            );



        }


        private static void DefineOrder(BundleCollection bundles)
        {
            //+ORDER MANAGEMENT
            bundles.FileSetOrderList.Clear();

            BundleFileSetOrdering ordering = new BundleFileSetOrdering("order");
            ordering.Files.Add("jquery.js");
            ordering.Files.Add("jquery.validate.js");
            ordering.Files.Add("popper.js");
            ordering.Files.Add("bootstrap.js");

            bundles.FileSetOrderList.Add(ordering);
        }
    }
}
