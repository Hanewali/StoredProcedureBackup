# StoredProcedureBackup
Small project for automatic running for systems where stored procedures aren't under version control. It will download alter scripts and put them in *.sql files in git repository.

This project is probably unnecesarry for most developers, but in my Software House, we don't have any version control for stored procedures or table/scalar sql functions. Because I've got enough of people overriting my changes accidentaly, in database-stored procedures, i decided to create small app to solve this problem. It doesn't save sql functions (yet?) but it will help a lot.

# Instructions
Short one: After publish, create directory "StoredProcedures" in app folder, and initialize git repository in it. It has to save them somewhere, doesn't it?
