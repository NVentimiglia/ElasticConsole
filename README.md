# ElasticConsole
ElasticConsole Nest FanOutRead Experiment 

// download and extract
https://www.elastic.co/downloads/elasticsearch

// dir
cd C:\elasticsearch-5.2.2\bin

// start
elasticsearch -Ecluster.name=nventi -Enode.name=nventi

// ping
GET http://localhost:9200/ (postman)

// health
http://localhost:9200/_cat/health?v
http://localhost:9200/_cat/nodes?v&pretty

// List all Indicies
localhost:9200/_cat/indices?v&pretty

// Create Index
PUT localhost:9200/news?pretty
GET localhost:9200/_cat/indices?v&pretty

//
400002 total took 258ms for 100
100000 total took 48ms for 100
