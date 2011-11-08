using System;
using System.Web.Mvc;
using System.Web.Routing;

using SampleSolrApp.Controllers;

using StructureMap;

namespace SampleSolrApp.Core.Ioc
{
    public class StructureMapControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext context, Type controllerType)
        {
            //if (controllerType == null)
            //    RedirectManager.RedirectToNotFound();

            try
            {
                return ObjectFactory.GetInstance(controllerType) as Controller;
            }
            catch (StructureMapException)
            {
                System.Diagnostics.Debug.WriteLine(ObjectFactory.WhatDoIHave());
                throw;
            }
            catch (Exception)
            {
                if (context.RouteData.Values.ContainsKey("controller"))
                    context.RouteData.Values["controller"] = "Home";
                else
                    context.RouteData.Values.Add("controller", "Home");

                if (context.RouteData.Values.ContainsKey("action"))
                    context.RouteData.Values["action"] = "NotFound";
                else
                    context.RouteData.Values.Add("action", "NotFound");

                return ObjectFactory.GetInstance(typeof(HomeController)) as Controller;
            }
        }
    }
}