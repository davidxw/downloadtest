using Common;
using DownloadTestUI.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace DownloadTestUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private IInternetService internetService;
        private ISettingsStore settingsStore;

        private string hostName;

        private TestResult lastResult;
        public TestResult LastResult
        {
            get
            {
                return lastResult;
            }
            set
            {
                lastResult = value;
                RaisePropertyChanged(()=>LastResult);
                RaisePropertyChanged(() => LastResult.mbps);
            }

        }

        private ObservableCollection<TestResult> testResults;
        public ObservableCollection<TestResult> TestResults
        {
            get
            {
                return testResults;
            }
            private set
            {
                testResults = value;
                RaisePropertyChanged(() => TestResults);
            }
        }

        private Uri downloadUri;
        public string DownloadUrl
        {
            get
            {
                return settingsStore.DownloadUrl;
            }
            set
            {
                settingsStore.DownloadUrl = value;
                RaisePropertyChanged(() => DownloadUrl);

                IsDownloadUrlValid = Uri.TryCreate(DownloadUrl, UriKind.Absolute, out downloadUri);

            }
        }

        private bool isDownloadUrlValid;
        public bool IsDownloadUrlValid
        {
            get
            {
                return isDownloadUrlValid;
            }
            set
            {
                isDownloadUrlValid = value;
                RaisePropertyChanged(()=> IsDownloadUrlValid);
            }
        }

        private Uri resultsUri;
        public string ResultsUrl
        {
            get
            {
                return settingsStore.ResultsUrl;
            }
            set
            {
                settingsStore.ResultsUrl = value;
                RaisePropertyChanged(() => ResultsUrl);

                IsResultsUrlValid = Uri.TryCreate(ResultsUrl, UriKind.Absolute, out resultsUri);
            }
        }

        private bool isResultsUrlValid;
        public bool IsResultsUrlValid
        {
            get
            {
                return isResultsUrlValid;
            }
            set
            {
                isResultsUrlValid = value;
                RaisePropertyChanged(() => IsResultsUrlValid);
            }
        }

        public TimeSpan TestInterval
        {
            get
            {
                return settingsStore.TestInterval;
            }
            set
            {
                settingsStore.TestInterval = value;
                RaisePropertyChanged(() => TestInterval);
            }
        }

        public RelayCommand StartTestingCommand
        {
            get;
            private set;
        }

        public RelayCommand StopTestingCommand
        {
            get;
            private set;
        }

        public RelayCommand TestNowCommand
        {
            get;
            private set;
        }

        public MainViewModel(IInternetService internetService, ISettingsStore settingsStore)
        {
            hostName = GetHostName();

            TestResults = new ObservableCollection<TestResult>();

            this.internetService = internetService;
            this.settingsStore = settingsStore;

            StartTestingCommand = new RelayCommand(StartTesting);

            StopTestingCommand = new RelayCommand(StopTesting);

            TestNowCommand = new RelayCommand(TestNow);

            DownloadUrl = "http://members.optusnet.com.au/optusost2/pinball_9mb.jpg";
        }

        private void StartTesting()
        {
            throw new NotImplementedException();
        }

        private void StopTesting()
        {
            throw new NotImplementedException();
        }

        private async void TestNow()
        {
            await RunOnce();
        }

        private async Task RunOnce()
        {
            TestResult testResult = null;

            if (!string.IsNullOrEmpty(DownloadUrl) && IsDownloadUrlValid)
            {
                testResult = await internetService.Get(downloadUri);

                LastResult = testResult;

                TestResults.Add(testResult);
            }

            if (!string.IsNullOrEmpty(ResultsUrl) && IsResultsUrlValid)
            {
                var resultsPostResponse = await internetService.PostResult(resultsUri, testResult, hostName);
            }

        }

        public string GetHostName()
        {
            var hostNames = NetworkInformation.GetHostNames();
            var hostName = hostNames.FirstOrDefault(name => name.Type == HostNameType.DomainName)?.DisplayName ?? "unkownHost";

            return hostName;
        }
    }
}
