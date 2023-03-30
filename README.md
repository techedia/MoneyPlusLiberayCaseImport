# MoneyPlusLiberayCaseImport
MoneyPlusLiberayCaseImport

## Purpose

Read in a master CSV file and produce 2 seperate import CSV files for the import of Liberay cases into Proclaim, the seperate CSV import files are delivered into 2 seperate folders to be imporeted from

In principle this is a generic solution to achieve likewise for any import process as long as the master CSV file being processed doesn't need to be prepped 


##  Fuctionality\Process

* Check we are within operational hours - to ensure import files are setup within operational hours of Proclaim (DB \ task servers)
* Check no existing import files exist - if they do then Proclaim is still importing cases so wait
* Make a backup to a folder of the master CSV file before further processing
* Get the first x rows from the master CSV file for the import file 1
* Get the next x rows from the master CSV file for the import file 2
* Remove the rows from the master CSV file that are going into import file 1 and 2
* Create the impoer files 1 and 2

## Config

Self documenting and setting should be obvious to what they are for
