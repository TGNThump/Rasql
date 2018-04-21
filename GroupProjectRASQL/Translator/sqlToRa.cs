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

            String returnString ="";

            //If there is no asterisk add a projection
            bool isProjection = node.Child(0).Child(1).Child(0).Data != "*<br />";

            if (isProjection) {

                returnString += "π ";
                returnString +=  TranslateAttributeList(node.Child(0).Child(1));
                returnString += " (";

            }

            //If there is a where clause, then add a select
            bool isWhere = node.Children.Count() == 5 && node.Child(4).Data == "[where]<br />";

            if (isWhere) {

                returnString += "σ ";
                returnString += node.Child(4).Child(1).TreeToString();
                returnString += " (";
            }


            returnString += TranslateFrom(node.Child(2));

            //Close open brackets
            if (isWhere) returnString += ")";

            if (isProjection) returnString += ")";

            return returnString;

        }

        public static String TranslateAttributeList(Node node) {

            return "";

        }

        public static String TranslateFrom(Node node) {

            return "";

        }

    }
}
