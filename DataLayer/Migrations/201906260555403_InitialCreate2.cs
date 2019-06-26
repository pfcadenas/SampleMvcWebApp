namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate2 : DbMigration
    {
        public override void Up()
        {
            //Function to split a text in separated words
            Sql(@"SET ansi_nulls ON
                GO

                SET quoted_identifier ON
                GO

                CREATE FUNCTION [dbo].[StrSplit] (@string NVARCHAR(max), @delimiter CHAR(1))
                  returns @output TABLE(splitdata NVARCHAR(max))
                  BEGIN
                    DECLARE @start INT,
                    @end   INT

                    SELECT @start = 1,

                    @end = Charindex(@delimiter, @string)

                    WHILE @start < Len(@string) + 1
                      BEGIN
                        IF @end = 0
                        SET @end = Len(@string) + 1

                        INSERT INTO @output
                          (splitdata)
                          VALUES (Substring(@string, @start, @end - @start))

                        SET @start = @end + 1
                        SET @end = Charindex(@delimiter, @string, @start)
                      END

                    RETURN
                  END
                  GO");

            //Function to remove all character except letters
            Sql(@"CREATE FUNCTION dbo.RemoveChars(@cadena varchar(1000))
                    RETURNS VARCHAR(1000)
                    BEGIN
                      DECLARE @pos INT
                      SET @Pos = PATINDEX('%[^abcdefghijlmnÒopqrstuvwxyzABCDEFGHIJKLMN—OPQRSTUVWXYZ·ÈÌÛ˙Ò—¡…Õ”⁄]%',@cadena)
                      WHILE @Pos > 0
                       BEGIN
                        SET @cadena = STUFF(@cadena,@pos,1,'')
                        SET @Pos = PATINDEX('%[^abcdefghijlmnÒopqrstuvwxyzABCDEFGHIJKLMN—OPQRSTUVWXYZ·ÈÌÛ˙Ò—¡…Õ”⁄]%',@cadena)
                       END
                      RETURN @cadena
                    END
                    GO");

            //Function to search all words in the content of an specific Post
            Sql(@"CREATE FUNCTION [dbo].[FindWordByPost] (@id INT)
                    RETURNS TABLE
                    AS
                    RETURN 
                      (
                      SELECT 
	                    splitdata AS word, 
	                    count(splitdata) AS weigth 
                      FROM ( 
		                SELECT 
			              dbo.RemoveChars(splitdata) AS splitdata
		                FROM StrSplit( 
			              (SELECT p.Content FROM dbo.Posts AS p WHERE p.PostId = @id) , ' ') ) AS s 	
                      WHERE 
                        lower(splitdata) NOT IN ('', ' ', 'i', 'am', 'you', 'she', 'he', 'they', 'yours', 'his', 'her', 'him', 'my',
                                                  'the', 'this', 'those', 'that', 'and', 'or', 'not', 'yes', 'as', 'is', 'on', 'in',
                                                   'of','to', 'from', 'at', 'for', 'a', 'an')
	                    AND splitdata NOT LIKE '%ar' AND splitdata NOT LIKE '%er' AND splitdata NOT LIKE '%ir'
                      GROUP BY splitdata 
                    );");


        }
        
        public override void Down()
        {
            //Drop all function if migrate to old version
            Sql(@"DROP FUNCTION dbo.[FindWordByPost];");
            Sql(@"DROP FUNCTION dbo.[StrSplit];");
            Sql(@"DROP FUNCTION dbo.[RemoveChars];");           
        }
    }
}
