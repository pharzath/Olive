﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olive.ApiProxy
{
    internal class ReadmeFileGenerator
    {
        internal static string Generate()
        {
            var r = new StringBuilder();
            r.AppendLine();

            r.AppendLine($"The {Context.ControllerType.FullName}.Proxy package is generated by « Olive Api Proxy Generator ».");
            r.AppendLine();

            r.AppendLine("To learn how to use it, visit: https://geeksltd.github.io/Olive/#/Api/Proxy");
            r.AppendLine();
            r.AppendLine();

            r.AppendLine(GenerateQuickReference());
            r.AppendLine(GenerateProxyConfig());
            r.AppendLine(GenerateDataProviders());

            return r.ToString();
        }

        static string GenerateQuickReference()
        {
            var r = new StringBuilder();
            r.AppendLine("HOW TO USE?");
            r.AppendLine("-------------------");
            r.AppendLine();

            var methods = Context.ActionMethods.Select(x => x.Method);

            foreach (var method in methods)
            {
                var returnType = method.GetApiMethodReturnType();
                if (returnType != null)
                    r.Append(returnType.GetProgrammingName() + " result = ");

                r.Append("await ");
                r.Append("new " + Context.ControllerType.FullName + "().");
                r.Append(method.Name);
                r.Append("(");
                r.Append(method.GetParameters().Select(x => "my" + x.Name).ToString(", "));
                r.AppendLine(");");

                r.AppendLine();
            }

            return r.ToString();
        }

        static string GenerateProxyConfig()
        {
            var r = new StringBuilder();
            r.AppendLine();
            r.AppendLine("PROXY CONFIGURATION OPTIONS:");
            r.AppendLine("-------------------");
            r.AppendLine("You can configure the proxy's behaviour when you create an Api proxy instance, before invoking the Api method.");
            r.AppendLine("For example:");
            r.AppendLine();
            r.AppendLine("new " + Context.ControllerType.FullName + "()");
            r.AppendLine("   .Retries(5)");
            r.AppendLine("   .Cache(CachePolicy.CacheOrFreshOrFail)");
            r.AppendLine("   .CircuitBreaker(exceptionsBeforeBreaking: 5, breakDurationSeconds: 10)");
            r.AppendLine("   ...;");
            r.AppendLine();
            return r.ToString();
        }

        static string GenerateDataProviders()
        {
            var dataProviders = DtoTypes.All
             .Where(x => new DtoDataProviderClassGenerator(x).Generate().HasValue()).ToList();

            if (dataProviders.None()) return null;

            var r = new StringBuilder();
            r.AppendLine("REMOTE DATA PROVIDER:");
            r.AppendLine("-------------------");
            r.AppendLine("If you want to create local entity associations to the following types, add the following in your application StartUp (and set your desired cache policy):");
            r.AppendLine();

            r.AppendLine($@"public override void Configure(IApplicationBuilder app, IHostingEnvironment env)");
            r.AppendLine("{");
            r.AppendLine("...");
            r.AppendLine();

            foreach (var type in dataProviders)
            {
                r.AppendLine("    " + type.FullName + "DataProvider");
                r.AppendLine("      .Register(x => x.Cache(CachePolicy.CacheOrFreshOrFail, cacheExpiry: 1.Minutes()));");
                r.AppendLine();
            }

            r.AppendLine("}");
            r.AppendLine();
            return r.ToString();
        }
    }
}