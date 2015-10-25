﻿/*
Copyright (c) 2012 <a href="http://www.gutgames.com">James Craig</a>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

#region Usings
using System;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Company.Utilities.DataTypes.ExtensionMethods;
using System.Globalization;
#endregion

namespace Company.Utilities.FileFormats
{
    /// <summary>
    /// Creates a VCalendar item
    /// </summary>
    public class VCalendar
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public VCalendar()
        {
            AttendeeList = new MailAddressCollection();
            Status = "BUSY";
            CurrentTimeZone = TimeZone.CurrentTimeZone;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The time zone for the calendar event
        /// </summary>
        public TimeZone CurrentTimeZone { get; set; }

        /// <summary>
        /// The start time
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The end time
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// The location of the event
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The subject of the item to send
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The description of the event
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// List of attendees
        /// </summary>
        public MailAddressCollection AttendeeList { get; private set; }

        /// <summary>
        /// Organizer
        /// </summary>
        public MailAddress Organizer { get; set; }

        /// <summary>
        /// Sets the status for the appointment (FREE, BUSY, etc.)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Determines if the calendar item is being canceled
        /// </summary>
        public bool Cancel { get; set; }

        private static readonly Regex STRIP_HTML_REGEX = new Regex("<[^>]*>", RegexOptions.Compiled);

        #endregion

        #region Private Functions

        private static string StripHTML(string HTML)
        {
            if (string.IsNullOrEmpty(HTML))
                return string.Empty;

            HTML = STRIP_HTML_REGEX.Replace(HTML, string.Empty);
            HTML = HTML.Replace("&nbsp;", " ");
            return HTML.Replace("&#160;", string.Empty);
        }

        private static bool ContainsHTML(string Input)
        {
            if (string.IsNullOrEmpty(Input))
                return false;

            return STRIP_HTML_REGEX.IsMatch(Input);
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Returns the VCalendar item
        /// </summary>
        /// <returns>a string output of the VCalendar item</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual string GetVCalendar()
        {
            return new StringBuilder().AppendLine("BEGIN:VCALENDAR")
                      .AppendLine("VERSION:1.0")
                      .AppendLine("BEGIN:VEVENT")
                      .AppendLineFormat("DTStart:{0}", CurrentTimeZone.ToUniversalTime(StartTime).ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture))
                      .AppendLineFormat("DTEnd:{0}", CurrentTimeZone.ToUniversalTime(EndTime).ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture))
                      .AppendLineFormat("Location;ENCODING=QUOTED-PRINTABLE:{0}", Location)
                      .AppendLineFormat("SUMMARY;ENCODING=QUOTED-PRINTABLE:{0}", Subject)
                      .AppendLineFormat("DESCRIPTION;ENCODING=QUOTED-PRINTABLE:{0}", Description)
                      .AppendLineFormat("UID:{0}{1}{2}", CurrentTimeZone.ToUniversalTime(StartTime).ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture), CurrentTimeZone.ToUniversalTime(EndTime).ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture), Subject)
                      .AppendLine("PRIORITY:3")
                      .AppendLine("End:VEVENT")
                      .AppendLine("End:VCALENDAR")
                      .ToString();
        }

        /// <summary>
        /// Returns the ICalendar item
        /// </summary>
        /// <returns>a string output of the ICalendar item</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual string GetICalendar()
        {
            StringBuilder FileOutput = new StringBuilder();
            FileOutput.AppendLine("BEGIN:VCALENDAR")
                      .AppendLineFormat("METHOD:{0}", Cancel ? "CANCEL" : "REQUEST")
                      .AppendLine("PRODID:-//Craigs Utility Library//EN")
                      .AppendLine("VERSION:2.0")
                      .AppendLine("BEGIN:VEVENT")
                      .AppendLine("CLASS:PUBLIC")
                      .AppendLineFormat("DTSTAMP:{0}", DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                      .AppendLineFormat("CREATED:{0}", DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                      .AppendLine(StripHTML(Description.Replace("<br />", "\\n")))
                      .AppendLineFormat("DTStart:{0}", CurrentTimeZone.ToUniversalTime(StartTime).ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                      .AppendLineFormat("DTEnd:{0}", CurrentTimeZone.ToUniversalTime(EndTime).ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                      .AppendLineFormat("LOCATION:{0}", Location)
                      .AppendLineFormat("SUMMARY;LANGUAGE=en-us:{0}", Subject)
                      .AppendLineFormat("UID:{0}{1}{2}", CurrentTimeZone.ToUniversalTime(StartTime).ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture), CurrentTimeZone.ToUniversalTime(EndTime).ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture), Subject);
            if (AttendeeList.Count > 0)
                FileOutput.AppendLineFormat("ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE;CN=\"{0}\":MAILTO:{0}", AttendeeList.ToString());
            if (Organizer != null)
                FileOutput.AppendLineFormat("ACTION;RSVP=TRUE;CN=\"{0}\":MAILTO:{0}\r\nORGANIZER;CN=\"{1}\":mailto:{0}", Organizer.Address, Organizer.DisplayName);
            if (ContainsHTML(Description))
            {
                FileOutput.AppendLineFormat("X-ALT-DESC;FMTTYPE=text/html:{0}", Description.Replace("\n", ""));
            }
            else
            {
                FileOutput.AppendLineFormat("DESCRIPTION:{0}", Description);
            }
            return FileOutput.AppendLine("SEQUENCE:1")
                             .AppendLine("PRIORITY:5")
                             .AppendLine("CLASS:")
                             .AppendLineFormat("LAST-MODIFIED:{0}", DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                             .AppendLine("STATUS:CONFIRMED")
                             .AppendLine("TRANSP:OPAQUE")
                             .AppendLineFormat("X-MICROSOFT-CDO-BUSYSTATUS:{0}",Status)
                             .AppendLine("X-MICROSOFT-CDO-INSTTYPE:0")
                             .AppendLine("X-MICROSOFT-CDO-INTENDEDSTATUS:BUSY")
                             .AppendLine("X-MICROSOFT-CDO-ALLDAYEVENT:FALSE")
                             .AppendLine("X-MICROSOFT-CDO-IMPORTANCE:1")
                             .AppendLine("X-MICROSOFT-CDO-OWNERAPPTID:-1")
                             .AppendLineFormat("X-MICROSOFT-CDO-ATTENDEE-CRITICAL-CHANGE:{0}", DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                             .AppendLineFormat("X-MICROSOFT-CDO-OWNER-CRITICAL-CHANGE:{0}", DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                             .AppendLine("BEGIN:VALARM")
                             .AppendLine("TRIGGER;RELATED=START:-PT00H15M00S")
                             .AppendLine("ACTION:DISPLAY")
                             .AppendLine("DESCRIPTION:Reminder")
                             .AppendLine("END:VALARM")
                             .AppendLine("END:VEVENT")
                             .AppendLine("END:VCALENDAR")
                             .ToString();
        }

        /// <summary>
        /// Returns the HCalendar item
        /// </summary>
        /// <returns>A string output of the HCalendar item</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual string GetHCalendar()
        {
            StringBuilder Output = new StringBuilder();
            Output.Append("<div class=\"vevent\">")
                  .Append("<div class=\"summary\">").Append(Subject).Append("</div>")
                  .Append("<div>Date: <abbr class=\"dtstart\" title=\"")
                  .Append(StartTime.ToString("MM-dd-yyyy hh:mm tt", CultureInfo.InvariantCulture)).Append("\">")
                  .Append(StartTime.ToString("MMMM dd, yyyy hh:mm tt", CultureInfo.InvariantCulture)).Append("</abbr> to ")
                  .Append("<abbr class=\"dtend\" title=\"").Append(EndTime.ToString("MM-dd-yyyy hh:mm tt", CultureInfo.InvariantCulture))
                  .Append("\">");
            if (EndTime.Year != StartTime.Year)
            {
                Output.Append(EndTime.ToString("MMMM dd, yyyy hh:mm tt", CultureInfo.CurrentCulture));
            }
            else if (EndTime.Month != StartTime.Month)
            {
                Output.Append(EndTime.ToString("MMMM dd hh:mm tt", CultureInfo.CurrentCulture));
            }
            else if (EndTime.Day != StartTime.Day)
            {
                Output.Append(EndTime.ToString("dd hh:mm tt", CultureInfo.CurrentCulture));
            }
            else
            {
                Output.Append(EndTime.ToString("hh:mm tt", CultureInfo.CurrentCulture));
            }
            return Output.Append("</abbr></div>")
                         .Append("<div>Location: <span class=\"location\">").Append(Location).Append("</span></div>")
                         .Append("<div class=\"description\">").Append(Description).Append("</div>")
                         .Append("</div>")
                         .ToString();
        }

        /// <summary>
        /// Returns the text version of the appointment
        /// </summary>
        /// <returns>A text version of the appointement</returns>
        public override string ToString()
        {
            return "Type:Single Meeting\r\n" +
                "Organizer:" + Organizer.DisplayName + "\r\n" +
                "Start Time:" + StartTime.ToLongDateString() + " " + StartTime.ToLongTimeString() + "\r\n" +
                "End Time:" + EndTime.ToLongDateString() + " " + EndTime.ToLongTimeString() + "\r\n" +
                "Time Zone:" + System.TimeZone.CurrentTimeZone.StandardName + "\r\n" +
                "Location: " + Location + "\r\n\r\n" +
                "*~*~*~*~*~*~*~*~*~*\r\n\r\n" +
                Description;
        }

        #endregion
    }
}