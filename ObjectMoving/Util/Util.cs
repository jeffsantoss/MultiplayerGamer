using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectMoving.Utils
{
    public static class Util
    {
        public static void HideAndShow(this Form formToHide, Form formToShow ){
                formToHide.Hide();
                formToShow.Show();
        }
    }
}
