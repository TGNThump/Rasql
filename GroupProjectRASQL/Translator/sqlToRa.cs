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

        public static String TranslateQuery(Node node) {

            String returnString = "";



            //If there is no asterisk add a projection
            bool isSelect = node.Child(0).Child(1).Child(0).Data != "*";

            Dictionary<String, String> attributeRenames = new Dictionary<string, string>();

            if (isSelect)
            {

                String selectList = TranslateSelectList(node.Child(0).Child(1), ref attributeRenames);

                foreach (KeyValuePair<String, String> entry in attributeRenames)
                {

                    returnString += "ρ " + entry.Value + "/" + entry.Key + " (";

                }

                returnString += "π " + selectList + " (";

            }

            //If there is a where clause, then add a select
            bool isWhere = node.Children.Count() == 5 && node.Child(4).Data == "[where]";

            if (isWhere)
            {

                returnString += "σ ";
                returnString += node.Child(4).Child(1).TreeToString();
                returnString += " (";
            }


            returnString += TranslateFromList(node.Child(2).Child(1));

            //Close open brackets
            if (isWhere) returnString += ")";
            if (isSelect) returnString += String.Concat(Enumerable.Repeat(")", 1 + attributeRenames.Count));

            return returnString;

        }

        public static String TranslateSelectList(Node node, ref Dictionary<String, String> renames) {

            String returnString = "";

            Node element = node.Child(0);

            String name = element.Child(0).TreeToString();

            bool isRename = element.Children.Count() == 3;
            if (isRename)
            {


                String newName = element.Child(2).TreeToString();

                renames.Add(name, newName);

            }


            returnString += name;

            if (node.Children.Count() == 3)
            {

                returnString += ",";
                returnString += TranslateSelectList(node.Child(2), ref renames);

            }

            return returnString;

        }

        public static String TranslateFromList(Node node) {

            String returnString = "";

            bool isCartesian = node.Children.Count() == 3;

            if (isCartesian) returnString += "X(";

            returnString += TranslateFromElement(node.Child(0));

            if (isCartesian) {

                returnString += ", ";
                returnString += TranslateFromList(node.Child(2));
                returnString += ")";


            }

            return returnString;

        }

        public static String TranslateFromElement(Node node) {

            String returnString = "";

            if (node.Children.Count() == 3)
            {

                returnString += "ρ " + node.Child(2).TreeToString() + " (";
                returnString += TranslateFromElement(node.Child(0));
                returnString += ")";

            }
            else if (node.Children.Count() == 4)
            {

                returnString += "ρ " + node.Child(3).TreeToString() + " (";
                returnString += TranslateQuery(node.Child(1));
                returnString += ")";

            }
            else if (node.Child(0).Data == "[join]")
            {

                returnString += TranslateJoin(node.Child(0));

            }
            else {

                returnString = node.TreeToString();

            }

            return returnString;

        }

        public static String TranslateJoin(Node node) {

            String returnString = "⋈ ";

            returnString += node.Child(4).TreeToString() + " (";
            returnString += TranslateFromElement(node.Child(0)) + ", ";
            returnString += TranslateFromElement(node.Child(2)) + ")";

            return returnString;
            
        }

    }
}
