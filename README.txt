SampleSolrApp

Steps to run sample application

1. Using SolrNet NHibernate integration
 * create database SampleSolrApp on your local SQLExpress
 * edit solr-1.4.0\start.bat to set correct path to samplesolrnet\solr-1.4.0\solr\multicore\
 * run solr-1.4.0\start.bat
 * start SampleSolrApp using VS.NET, should run as http://localhost:25827/
 * SampleSolrApp will create db schema and setup solr index
 * run Simulate_10_Users_Should_Crash_Application test from SampleSolrApp.Tests\SimulateMultipleUsersTester.cs using NUnit
 * test will simulate 10 users, every user makes 20 request to http://localhost:25827/home/add -> this action is inserting Products to db and should add documents to Solr
 * we should have 200 products in db and 200 documents in Solr
 * unfortunately we get about 30 errors, which means we get about 170 products in db, but only about 130 product documents in Solr
 
2. Using own simple NHibernate listener
 * comment out lines in SampleSolrApp.Core.NhInfrastructure.NHibernateRegistry between // Solr & NHibernate integration -> and // <- Solr & NHibernate integration
 * comment in lines in SampleSolrApp.Core.NhInfrastructure.NHibernateRegistry between // manual NHibernate integration -> and // <- manual NHibernate integration 
 * stop the dev web server
 * start SampleSolrApp using VS.NET, should run as http://localhost:25827/
 * run Simulate_10_Users_Should_Crash_Application test from SampleSolrApp.Tests\SimulateMultipleUsersTester.cs using NUnit
 * test will simulate 10 users, every user makes 20 request to http://localhost:25827/home/add -> this action is inserting Products to db and should add documents to Solr
 * we should have 200 products in db and 200 documents in Solr
 * we get 200 products and 200 documents in Solr, we get few different error, which don't break db and Solr index integrity