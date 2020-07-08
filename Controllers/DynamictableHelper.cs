using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppWebServer.Controllers
{
    public class DynamictableHelper
    {
      static  public string GetDynamictable(List<string[]> value, List<string> title, string tableName)
        {
            string row = "";
            string titles = "";
            string rowTemple = "<td>{0}</td>";
            string rowbody = "";

            foreach (string h in title)
            {
                titles += string.Format("<th class='text-center'>{0}</th>", h);
            }
            int j = 0;
            for (; j < value.Count; j++)
            {
                row = "<td>" + (j + 1) + "</td>";
                for (int i = 0; i < title.Count; i++)
                {
                    row += string.Format("<td><input name='{0}' type='text' value='{1}' placeholder='{2}' class='form-control input-md'/> </td>", title[i] + j, (value[j])[i], title[i]);
                }

                rowbody += "<tr id='r" + tableName + j + "'>" + row + "</tr>";
            }

            for (int i = 0; i < title.Count; i++)
            {
                rowTemple += "<td><input name='" + title[i] + "{1}' type='text' value='' placeholder='" + title[i] + "' class='form-control input-md'/></td>";
            }
            string DynamicTableString =
                "<a id='add_row" + tableName + "' class='btn btn-default pull-left'>Add Row</a><a id = 'delete_row" + tableName + "' class='pull-right btn btn-default'>Delete Row</a><p></p>" +
                "<table class='table table-bordered table-hover' id='" + tableName + "'><thead>" +
                " <tr><th class='text-center'></th>" + titles + //loop here
                "</tr></thead>" +
                "<tbody>" + rowbody + "</tbody></table>" +
                "<script>tableDynamic(\"" + tableName + "\",\"" + rowTemple + "\",\"" + value.Count + "\");</script>";//function tableDynamic (id,rowString,indexInit) {
            return DynamicTableString;

        }
    }
}