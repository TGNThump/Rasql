using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupProjectRASQL.Parser;

using Node = GroupProjectRASQL.Parser.TreeNode<System.String>;


namespace GroupProjectRASQL.Translator
{
    class SqlToRa
    {

        //Takes a parse tree for a SQL query and return an equivalent RA string
        public static String TranslateQuery(Node node) {

            String returnString = "";



            //If there is an asterisk, there is no selection
            bool isSelect = node.Child(0).Child(1).Child(0).Data != "*";

            //Create dictionary to store renamed attributes and what they're renamed to
            Dictionary<String, String> attributeRenames = new Dictionary<string, string>();

            //If there is a select:
            if (isSelect)
            {

                //Translate the select list and store renames
                String selectList = TranslateSelectList(node.Child(0).Child(1), ref attributeRenames);

                //Add the renames to string
                foreach (KeyValuePair<String, String> entry in attributeRenames)
                {

                    returnString += "ρ " + entry.Value + "/" + entry.Key + " (";

                }

                //Add translated select list
                returnString += "π " + selectList + " (";

            }

            //If there is a where clause, then add a select
            bool isWhere = node.Children.Count() == 6 && node.Child(4).Data == "[where]";

            //If there is a where, add the where clause and the condition
            if (isWhere)
            {

                returnString += "σ ";
                returnString += node.Child(4).Child(1).TreeToString();
                returnString += " (";
            }


            //Add the translated from list
            returnString += TranslateFromList(node.Child(2).Child(1));

            //Close open brackets
            if (isWhere) returnString += ")";
            if (isSelect) returnString += String.Concat(Enumerable.Repeat(")", 1 + attributeRenames.Count));

            return returnString;

        }

        //Returns a translated RA equivalent to a select list node
        public static String TranslateSelectList(Node node, ref Dictionary<String, String> renames) {

            String returnString = "";

            Node element = node.Child(0);

            String name = element.Child(0).TreeToString();

            //If the select list contains a rename, add it to the dictionary
            bool isRename = element.Children.Count() == 3;
            if (isRename)
            {


                String newName = element.Child(2).TreeToString();

                renames.Add(name, newName);

            }

            returnString += name;


            //If there are three elements, element 0 is the first element of list, 1 is a comma and 2 is another select list
            if (node.Children.Count() == 3)
            {

                returnString += ",";
                //translate rest of list and att it to string
                returnString += TranslateSelectList(node.Child(2), ref renames);

            }

            return returnString;

        }

        //Takes a from list node and returns an equivalent ra string
        public static String TranslateFromList(Node node) {

            String returnString = "";

            //If there are 3 elements, there is a cartesian product between the first and last
            bool isCartesian = node.Children.Count() == 3;

            if (isCartesian) returnString += "X(";

            //Translate and add the from element to the list
            returnString += TranslateFromElement(node.Child(0));

            //Translate rest of list
            if (isCartesian) {

                returnString += ", ";
                returnString += TranslateFromList(node.Child(2));
                returnString += ")";


            }

            return returnString;

        }

        //Takes a from element node and returns equivalent RA string
        public static String TranslateFromElement(Node node) {

            String returnString = "";

            //3 elements in element means there is a table rename
            if (node.Children.Count() == 3)
            {

                returnString += "ρ " + node.Child(2).TreeToString() + " (";
                returnString += TranslateFromElement(node.Child(0));
                returnString += ")";

            }
            //4 elements: named subquery
            else if (node.Children.Count() == 4)
            {

                returnString += "ρ " + node.Child(3).TreeToString() + " (";
                returnString += TranslateQuery(node.Child(1));
                returnString += ")";

            }
            // join
            else if (node.Child(0).Data == "[join]")
            {

                returnString += TranslateJoin(node.Child(0));

            }
            //Otherwise, its just a relation
            else {

                returnString = node.TreeToString();

            }

            return returnString;

        }

        //Takes a join node and returns equivalent RA string
        public static String TranslateJoin(Node node) {

            String returnString = "⋈ ";

            returnString += node.Child(4).TreeToString() + " (";
            returnString += TranslateFromElement(node.Child(0)) + ", ";
            returnString += TranslateFromElement(node.Child(2)) + ")";

            return returnString;
            
        }

    }
}
