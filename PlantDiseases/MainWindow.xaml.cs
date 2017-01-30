using PlantDiseases.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlantDiseases
{
    /// <summary>
    /// Task：
    /// Collect all pages which URL like "https://www.daf.qld.gov.au/plants/..."
    /// Init URL https://www.daf.qld.gov.au/plants/
    /// Database:MySQL
    /// Use Entity Framework (Code First)
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string INITURL = "https://www.daf.qld.gov.au/plants/";

        private const string patternLinks = "(?<=\\shref=\\\")(https://www.daf.qld.gov.au/plants/).+?(?=\")";
        private const string patternTitle = "(?<=<title>).+?(?=</title>)";
        private const string patternDescription = "(?<=<meta\\s?name=\\\"description\\\"\\s?content=\\\").+?(?=\\\"\\s?/>)";
        private const string patternSubject = "(?<=<meta\\s?name=\\\"DC\\.Subject\\\"\\s.+?\\scontent=\\\").+?(?=\\\"\\s?/>)";
        private const string patternContent = "<(?<HtmlTag>[\\w]+)[^>]*\\sclass=(?<Quote>[\"']?)page\\-content(?(Quote)\\k<Quote>)[^>]*?(/>|>((?<Nested><\\k<HtmlTag>[^>]*>)|</\\k<HtmlTag>>(?<-Nested>)|.*?)*</\\k<HtmlTag>>)";
        //private const string patternContent = "<(?<HtmlTag>div)[^>]*?>((?<Nested><\\k<HtmlTag>[^>]*>)|</\\k<HtmlTag>>(?<-Nested>)|.*?)*</\\k<HtmlTag>>";

        private const int workersNum = 10;

        private BackgroundWorker worker;
        private PlantDiseasesDbContext db;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }



        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // init database
            // if TargetLink table is empty, insert InitUrl
            worker.ReportProgress(0, "Initialize");
            db = new PlantDiseasesDbContext();
            if (!db.TargetLinks.Any())
                db.TargetLinks.Add(new TargetLink
                {
                    Done = false,
                    Url = INITURL
                });
            db.SaveChanges();
            db.Dispose();

            while (true)
            {
                db = new PlantDiseasesDbContext();
                var target = db.TargetLinks.FirstOrDefault(t => !t.Done);
                if (target == null)// All target links have been visited, break the loop
                {
                    worker.ReportProgress(0, "All target links have been visited");
                    db.Dispose();
                    break;
                }

                HttpClient client = new HttpClient();
                try
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    worker.ReportProgress(0, target.Url);
                    List<TargetLink> newLinks = new List<TargetLink>();
                    var response = client.GetStringAsync(target.Url).Result; // Get Response HTML string

                    doc.LoadHtml(response);
                    var title = doc.DocumentNode.SelectSingleNode("//title");
                    var subject = doc.DocumentNode.SelectSingleNode("//meta[@name=\"DC.Subject\"]");
                    var description = doc.DocumentNode.SelectSingleNode("//meta[@name=\"description\"]");
                    var content = doc.DocumentNode.SelectSingleNode("//*[@class=\"page-content\"]");
                    HtmlPage page = new HtmlPage
                    {
                        Url = target.Url,
                        Title = title == null ? string.Empty : title.InnerHtml,
                        Subject = subject == null ? string.Empty : subject.Attributes["content"].Value,
                        Description = description == null ? string.Empty : description.Attributes["content"].Value,
                        Content = content == null ? string.Empty : content.InnerHtml,
                    };
                    worker.ReportProgress(0, page.Title + "\nDescription: " + page.Description + "\nSubject: " + page.Subject);

                    var links = Regex.Matches(response, patternLinks, RegexOptions.IgnoreCase);
                    if (links.Count > 0)
                    {
                        foreach (var link in links)
                        {
                            string strLink = link.ToString();
                            if (newLinks.Any(l => l.Url == strLink) || db.TargetLinks.Any(t => t.Url == strLink)) // if url is already in new link list or database, skip
                                continue;
                            newLinks.Add(new TargetLink
                            {
                                Done = false,
                                Url = link.ToString()
                            });
                        }
                    }
                    worker.ReportProgress(0, "New Links: " + newLinks.Count);

                    db.TargetLinks.AddRange(newLinks);// save new links(remove)
                    db.HtmlPages.Add(page); // save data
                    target.Done = true;     // mark current link 
                    db.Entry(target).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    if (ex.InnerException != null)
                        msg += "\n" + ex.InnerException.Message;
                    worker.ReportProgress(0, "ERROR: " + msg);
                    target.Done = true;     // mark current link 
                    target.Remarks = msg;
                    db.Entry(target).State = EntityState.Modified;
                    db.SaveChanges();
                }
                finally
                {
                    db.Dispose();

                }
            }

        }


        #region 进度显示和结束
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (listLog.Items.Count > 1000)
            {
                listLog.Items.RemoveAt(0);
            }

            listLog.Items.Add(e.UserState.ToString());
            listLog.SelectedIndex = listLog.Items.Count - 1;
            listLog.ScrollIntoView(listLog.SelectedItem);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listLog.Items.Add("=============== FINISH ===============\n\n\n");
        }
        #endregion

    }

}
