﻿// Enables the following conditional switch in the project options
// to prevent test cases from making any edits.
//          DRY_RUN

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static UnitTestProject1.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WikiClientLibrary;

namespace UnitTestProject1
{
    [TestClass]
    public class PageTestsDirty
    {
        private const string SummaryPrefix = "WikiClientLibrary test. ";

        private static Site site;

        // The following pages will be created.
        private const string TestPage1Title = "WCL test page 1";

        private const string TestPage11Title = "WCL test page 1/1";

        private const string TestPage2Title = "WCL test page 2";

        // The following pages will NOT be created at first.
        private const string TestPage12Title = "WCL test page 1/2";

        private static Page GetOrCreatePage(Site site, string title)
        {
            if (site == null) throw new ArgumentNullException(nameof(site));
            var page = new Page(site, title);
            AwaitSync(page.RefreshInfoAsync());
            if (!page.Exists)
            {
                Trace.WriteLine("Creating page: " + page);
                page.Content = $@"<big>This is a test page for '''WikiClientLibrary'''.</big>

This page is created by an automated program for unit test purposes.

If you see this page '''OUTSIDE''' a test wiki site,
maybe you should consider viewing the history of the page,
and find out who created the page accidentally.

The original title of the page is '''{title}'''.

== See also ==
* [[Special:PrefixIndex/{title}/|Subpages]]
";
                AwaitSync(page.UpdateContentAsync(SummaryPrefix + "Create test page for unit tests."));
            }
            return page;
        }

        [ClassInitialize]
        public static void OnClassInitializing(TestContext context)
        {
            AssertModify(); // We're doing dirty work in this calss.
            // Prepare test environment.
            site = CreateWikiSite(EntryPointWikipediaTest2);
            CredentialManager.Login(site);
            GetOrCreatePage(site, TestPage1Title);
            GetOrCreatePage(site, TestPage11Title);
            GetOrCreatePage(site, TestPage2Title);
        }

        [ClassCleanup]
        public static void OnClassCleanup()
        {
            CredentialManager.Logout(site);
        }

        [TestMethod]
        public void WpTest2PageMoveTest1()
        {
            var page = new Page(site, TestPage11Title);
            AwaitSync(page.MoveAsync(TestPage12Title, SummaryPrefix + "Move a page."));
        }
    }
}