using System.Web;
using System.Web.Optimization;

namespace Nostreets_Sandbox
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new StyleBundle("~/css").Include(

                      "~/assets/css/bootstrap.css",
                      "~/assets/css/bootstrap-theme.css",
                      "~/assets/css/theme-Dark.css",
                      "~/assets/css/material-dashboard.css",
                      "~/assets/css/chartist-plugin-tooltip.css",
                      "~/assets/css/Site.css",
                      

                      "~/assets/css/font-awesome.css",
                      "~/node_modules/angular-ui-bootstrap/dist/ui-bootstrap-csp.css",
                      "~/node_modules/angularjs-color-picker/dist/angularjs-color-picker.css",
                      "~/node_modules/angularjs-color-picker/dist/themes/angularjs-color-picker-bootstrap.min.css"
                      ));


            bundles.Add(new ScriptBundle("~/jsRequirements").Include(
                    "~/assets/js/jquery-3.2.1.js",
                    "~/assets/js/jquery.validate.js",
                    "~/assets/js/bootstrap.js",
                    "~/assets/js/material.min.js",
                    "~/assets/js/sweetalert2.js",
                    "~/assets/js/chartist.min.js",
                    "~/assets/js/chartist-plugin-tooltip.min.js",
                    "~/assets/js/chartist-plugin-zoom.min.js",
                    "~/Scripts/app/services/nostreetsChartistScrollPlugin.js"
                        ));

            bundles.Add(new ScriptBundle("~/angularRequirements").Include(

                    "~/node_modules/angular/angular.js",
                    "~/node_modules/angular-animate/angular-animate.js",
                    "~/node_modules/angular-route/angular-route.js",
                    "~/node_modules/angular-sanitize/angular-sanitize.js",
                    "~/node_modules/angular-cookies/angular-cookies.js",
                    "~/node_modules/angular-toastr/dist/angular-toastr.min.js",
                    "~/node_modules/angular-ui-bootstrap/dist/ui-bootstrap-tpls.js",
                    "~/node_modules/tinycolor2/dist/tinycolor-min.js",
                    "~/node_modules/angularjs-color-picker/dist/angularjs-color-picker.js"

                        ));


            bundles.Add(new ScriptBundle("~/customAngular").Include(
                  "~/Scripts/app/core/global.js",
                  "~/Scripts/app/core/extend.js",
                  "~/Scripts/app/services/alertService.js",
                  "~/Scripts/app/services/systemEvent.service.js",
                  "~/Scripts/app/configs/routeConfig.js",
                  "~/Scripts/app/services/sandboxService.js",
                  "~/Scripts/app/directives/signInDirective.js"
                       ));


            bundles.Add(new ScriptBundle("~/other").Include(
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
                    "~/assets/js/particles.js",
                    "~/assets/js/renderParticles.js",
                    "~/assets/js/startUpPageScript.js"
                      ));



        }
    }
}
