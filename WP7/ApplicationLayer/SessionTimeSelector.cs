using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using Clarity.Phone.Controls;

using MonkeySpace.Core;
/*
 * Source of the alphabet jump list
 * http://blogs.claritycon.com/blogs/kevin_marshall/archive/2010/10/06/wp7-quick-jump-grid-sample-code.aspx
 */
namespace MonkeySpace
{
    public class SessionTimeSelector : IQuickJumpGridSelector
    {
        public Func<object, IComparable> GetGroupBySelector()
        {
            return (s) => ((Session)s).TimeQuickJumpDisplay;
        }
        public Func<object, string> GetOrderByKeySelector()
        {
            return (s) => ((Session)s).Start.Ticks.ToString();
        }
        public Func<object, string> GetThenByKeySelector()
        { return (s) => (string.Empty); }
    }
}