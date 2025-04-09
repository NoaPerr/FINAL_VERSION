USE family;
--CREATE TABLE FamilyRelations (
--    Person_Id INT PRIMARY KEY,-- במידה ומדובר במזהה שאינו מספר ת.ז 
--    Personal_Name NVARCHAR(100),
--    Family_Name NVARCHAR(100),
--    Gender NVARCHAR(10),
--    Father_Id INT,
--    Mother_Id INT,
--    Spouse_Id INT
--);
C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\family.mdf

--INSERT INTO FamilyRelations (Person_Id, Personal_Name, Family_Name, Gender, Father_Id, Mother_Id, Spouse_Id)
--VALUES (10, 'Yoni', 'Levin', 'Male', 4, 3, 20);

--INSERT INTO FamilyRelations (Person_Id, Personal_Name, Family_Name, Gender, Father_Id, Mother_Id, Spouse_Id)
--VALUES (20, 'Yael', 'Levin', 'Female', 1, 2, 10);

--INSERT INTO FamilyRelations (Person_Id, Personal_Name, Family_Name, Gender, Father_Id, Mother_Id, Spouse_Id)
--VALUES (4, 'Avi', 'Levin', 'Male', NULL, NULL, 3);

--INSERT INTO FamilyRelations (Person_Id, Personal_Name, Family_Name, Gender, Father_Id, Mother_Id, Spouse_Id)
--VALUES (3, 'Michal', 'Levin', 'Female', NULL, NULL, 4);

--INSERT INTO FamilyRelations (Person_Id, Personal_Name, Family_Name, Gender, Father_Id, Mother_Id, Spouse_Id)
--VALUES (1, 'Shalom', 'Shafir', 'Male', NULL, NULL, 6);

--INSERT INTO FamilyRelations (Person_Id, Personal_Name, Family_Name, Gender, Father_Id, Mother_Id, Spouse_Id)
--VALUES (2, 'Shir', 'Shafir', 'Female', NULL, NULL, 1);

--INSERT INTO FamilyRelations (Person_Id, Personal_Name, Family_Name, Gender, Father_Id, Mother_Id, Spouse_Id)
--VALUES (21, 'Ori', 'Shafir', 'Male', 1, 2, NULL);

--CREATE TABLE FamilyRelations (
--    Person_Id INT,
--    Relative_Id INT,
--    Connection_Type VARCHAR(50),
--    PRIMARY KEY (Person_Id, Relative_Id, Connection_Type),
--    FOREIGN KEY (Person_Id) REFERENCES Personal_Info(Person_Id),
--    FOREIGN KEY (Relative_Id) REFERENCES Personal_Info(Person_Id)
--);

--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Connection_Type)
--VALUES (20, 10, 'Sister');

--CREATE TABLE RelationshipTypes (
--    Relationship_Type_Id INT PRIMARY KEY IDENTITY(1,1),
--    Relationship_Type_Name NVARCHAR(50) NOT NULL
--);


---- הוספת סוגי קשרים לטבלה RelationshipTypes
--INSERT INTO RelationshipTypes (Relationship_Type_Name)
--VALUES ('Father'), ('Mother'), ('Brother'), ('Sister'), ('Son'), ('Daughter'), ('Husband'), ('Wife');


  --CREATE TABLE FamilyRelations (
  --  Person_Id INT,
  --  Relative_Id INT,
  --  Relationship_Type_Id INT,
  --  PRIMARY KEY (Person_Id, Relative_Id, Relationship_Type_Id),
  --  FOREIGN KEY (Person_Id) REFERENCES Personal_Info(Person_Id),
  --  FOREIGN KEY (Relative_Id) REFERENCES Personal_Info(Person_Id),
  --  FOREIGN KEY (Relationship_Type_Id) REFERENCES RelationshipTypes(Relationship_Type_Id)
--);

SELECT * FROM RelationshipTypes;

SELECT * FROM Personal_Info;

--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (1, 2, 7);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (1, 20, 1);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (2, 20, 2);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (2, 1, 8);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (3, 4, 8);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (3, 10, 2);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (4, 3, 7);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (4, 10, 1);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (10, 20, 7);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (10, 4, 5);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (10, 3, 5);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (20, 1, 6);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (20, 2, 6);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (20, 21, 4);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (21, 20, 3);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (21, 1, 5);
--INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
--VALUES (21, 2, 5);



--CREATE PROCEDURE AddFamilyRelationSimple
--    @PersonID INT,
--    @RelativeID INT,
--    @RelationTypeID INT
--AS
--BEGIN
--    INSERT INTO FamilyRelations (Person_ID, Relative_Id,Relationship_Type_Id)
--    VALUES (@PersonID, @RelativeID, @RelationTypeID)
--END
--GO




-- Create a temporary table to 
CREATE TABLE #TempSpouses (
    Person_Id INT,
    Relative_Id INT,
    Relationship_Type_Id INT
);
USE family;
-- store all pairs with Relationship_Type_Id 7,8 
INSERT INTO #TempSpouses (Person_Id, Relative_Id, Relationship_Type_Id)
SELECT Person_Id, Relative_Id, Relationship_Type_Id
FROM FamilyRelations
WHERE Relationship_Type_Id IN (7, 8);  

SELECT * FROM #TempSpouses;

-- Check for each row in the temporary table if the opposite row exists.
DECLARE @PersonId INT, @RelativeId INT, @RelationshipTypeId INT;

DECLARE db_cursor CURSOR FOR
SELECT Person_Id, Relative_Id, Relationship_Type_Id
FROM #TempSpouses;

OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @PersonId, @RelativeId, @RelationshipTypeId;

WHILE @@FETCH_STATUS = 0
BEGIN
   
    IF NOT EXISTS (SELECT 1
                   FROM FamilyRelations
                   WHERE Person_Id = @RelativeId
                     AND Relative_Id = @PersonId
                     AND Relationship_Type_Id = CASE 
                                                   WHEN @RelationshipTypeId = 7 THEN 8 
                                                   WHEN @RelationshipTypeId = 8 THEN 7 
                                                   END)
    BEGIN
        -- Add it in case the opposite row does not exist
        INSERT INTO FamilyRelations (Person_Id, Relative_Id, Relationship_Type_Id)
        VALUES (@RelativeId, @PersonId, CASE 
                                           WHEN @RelationshipTypeId = 7 THEN 8  -- בעל -> אישה
                                           WHEN @RelationshipTypeId = 8 THEN 7  -- אישה -> בעל
                                           END);
    END

    FETCH NEXT FROM db_cursor INTO @PersonId, @RelativeId, @RelationshipTypeId;
END

CLOSE db_cursor;
DEALLOCATE db_cursor;
DROP TABLE #TempSpouses;

