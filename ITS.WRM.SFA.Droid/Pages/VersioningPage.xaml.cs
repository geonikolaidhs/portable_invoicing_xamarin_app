using Acr.UserDialogs;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Constants;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VersioningPage : ContentPage
    {
        List<Type> DownloadTypes = new List<Type>();
        List<string> NameTypes = new List<string>();
        List<TableVersionPresent> PresentVersions = new List<TableVersionPresent>();
        private TableVersion SelectedTableVersion = null;
        private static System.Timers.Timer pageTimer;


        public VersioningPage()
        {
            InitializeComponent();
            btnSave.Text = ResourcesRest.Save;
            btnCancel.Text = ResourcesRest.Cancel;
            EditStack.IsVisible = false;
            VersionTableStack.IsVisible = true;
            DownloadTypes = DependencyService.Get<ICrossPlatformMethods>().GetTypesToUpdate().OrderBy(x => x.Name).ToList();
            NameTypes = DownloadTypes.Select(typ => typ.Name).ToList();
            PresentVersions = GeTableVersionsPresent();
            UpdateViewList();

        }

        private List<TableVersion> GetLocalTableVersions()
        {
            return App.DbLayer.GetTableVersions() ?? new List<TableVersion>();
        }
        private List<TableVersionPresent> GeTableVersionsPresent()
        {
            List<TableVersionPresent> presentver = new List<TableVersionPresent>();
            List<TableVersion> versions = GetLocalTableVersions();
            foreach (TableVersion ver in versions)
            {
                TableVersionPresent present = new TableVersionPresent();
                present.Oid = ver.Oid;
                present.Order = DownloadTypes?.Where(x => x.Name == ver.TableName)?.FirstOrDefault()?.GetCustomAttributes<CreateOrUpdaterOrderAttribute>()?.FirstOrDefault()?.Order ?? -1;
                present.TableName = ver.TableName;
                present.UpdatedOnticks = ver.UpdatedOnticks;
                present.Version = ver.Version;
                presentver.Add(present);
            }
            return presentver;
        }

        private void UpdateViewList()
        {
            VersionList.ItemsSource = new List<TableVersionPresent>();
            VersionList.ItemsSource = PresentVersions.OrderBy(x => x.TableName);

        }

        private async Task<long> GeServerVersionAsync(Type type)
        {
            return await DependencyService.Get<ICrossPlatformMethods>().GetServerMaxVersionAsync(type.Name);
        }

        protected async void OnGetServerVersion(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.SearchingDataPleaseWait, MaskType.Black);

                await Task.Run(async () =>
                {
                    foreach (Type type in DownloadTypes)
                    {
                        long ver = await GeServerVersionAsync(type);
                        TableVersionPresent tvp = PresentVersions.Where(x => x.TableName == type.Name).FirstOrDefault();
                        if (tvp != null)
                        {
                            tvp.ServerVersion = ver;
                        }
                    }

                });
                UpdateViewList();
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        async void OnSave(object sender, EventArgs args)
        {
            try
            {
                SelectedTableVersion.Version = Version.Date.Ticks;
                App.DbLayer.Update(SelectedTableVersion, typeof(TableVersion));
                SelectedTableVersion = null;
                RefreshListView();
                VersionTableStack.IsVisible = true;
                EditStack.IsVisible = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                UserDialogs.Instance.HideLoading();
            }
        }

        async void OnCancel(object sender, EventArgs args)
        {
            SelectedTableVersion = null;
            VersionTableStack.IsVisible = true;
            EditStack.IsVisible = false;

        }

        async void OnEdit(object sender, EventArgs args)
        {

            MenuItem item = (MenuItem)sender;
            Guid Oid;
            if (Guid.TryParse(item.CommandParameter.ToString(), out Oid))
            {
                SelectedTableVersion = App.DbLayer.GetTableVersionById(Oid);
                if (SelectedTableVersion != null)
                {
                    VersionTableStack.IsVisible = false;
                    EditStack.IsVisible = true;
                    TableName.Text = SelectedTableVersion.TableName;
                    Version.Date = new DateTime(SelectedTableVersion.Version);

                }
            }
        }

        private void RefreshListView()
        {
            List<TableVersionPresent> list = GeTableVersionsPresent();
            foreach (TableVersionPresent present in list)
            {
                TableVersionPresent tvp = PresentVersions.Where(x => x.Oid == present.Oid).FirstOrDefault();
                if (tvp != null)
                {
                    present.ServerVersion = tvp.ServerVersion;
                }
            }
            PresentVersions = new List<TableVersionPresent>();
            PresentVersions = list;
            UpdateViewList();
        }

        async void OnSync(object sender, EventArgs args)
        {
            MenuItem item = (MenuItem)sender;
            Guid Oid;
            if (Guid.TryParse(item.CommandParameter.ToString(), out Oid))
            {
                try
                {
                    SelectedTableVersion = App.DbLayer.GetTableVersionById(Oid);
                    if (SelectedTableVersion != null)
                    {
                        Type type = DownloadTypes.Where(x => x.Name == SelectedTableVersion.TableName).FirstOrDefault();
                        if (type != null)
                        {
                            SetTimer();
                            UserDialogs.Instance.Progress("Syncing", CancelAction, ResourcesRest.Cancel, true, MaskType.Black);
                            await DependencyService.Get<ICrossPlatformMethods>().SyncTableAsync(type);
                            pageTimer.Stop();
                            RefreshListView();
                            SelectedTableVersion = null;
                            UserDialogs.Instance.Progress().Hide();
                            await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.msgSuccessSynchronize, ResourcesRest.MsgBtnOk);
                        }
                        else
                        {
                            throw new Exception("Type nOt Found");
                        }
                    }
                }
                catch (Exception ex)
                {
                    pageTimer.Stop();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + "  " + ex.Message, ResourcesRest.MsgBtnOk);
                        UserDialogs.Instance.Progress().Hide();
                    });
                }
            }
        }

        private void CancelAction()
        {
            pageTimer.Stop();
            DependencyService.Get<ICrossPlatformMethods>().CancelSync();
            UserDialogs.Instance.Progress().Hide();
            RefreshListView();
        }

        private void SetTimer()
        {

            pageTimer = new System.Timers.Timer(4000);
            pageTimer.Elapsed += OnTimedEvent;
            pageTimer.AutoReset = true;
            pageTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            long ver = DependencyService.Get<ICrossPlatformMethods>().GetCurrentUpdatingVersion();
            if (ver != -1)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.Progress("Syncing " + new DateTime(ver).ToString(), CancelAction, ResourcesRest.Cancel, true, MaskType.Black);
                });
            }
        }


        protected void OnBackCliked(object sender, EventArgs e)
        {
            pageTimer.Stop();
        }


    }
}
