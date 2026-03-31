using System.Collections.ObjectModel;
using System.Windows;
using TinyUrl.WPF.Models;
using TinyUrl.WPF.Services;

namespace TinyUrl.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly UrlService _svc;

        // ─── Properties ───────────────────────────────────────

        private string _longUrl = "";
        public string LongUrl
        {
            get => _longUrl;
            set
            {
                SetProperty(ref _longUrl, value);
                UrlError = ValidateUrl(value);
            }
        }

        private string _urlError = "";
        public string UrlError
        {
            get => _urlError;
            set
            {
                SetProperty(ref _urlError, value);
                OnPropertyChanged(nameof(HasUrlError));
            }
        }

        public bool HasUrlError => !string.IsNullOrEmpty(UrlError);

        private bool _isPrivate;
        public bool IsPrivate
        {
            get => _isPrivate;
            set => SetProperty(ref _isPrivate, value);
        }

        private string _search = "";
        public string Search
        {
            get => _search;
            set
            {
                SetProperty(ref _search, value);
                _ = LoadUrlsAsync(); // auto search on type
            }
        }

        private string _generatedUrl = "";
        public string GeneratedUrl
        {
            get => _generatedUrl;
            set
            {
                SetProperty(ref _generatedUrl, value);
                OnPropertyChanged(nameof(HasGeneratedUrl));
            }
        }

        public bool HasGeneratedUrl => !string.IsNullOrEmpty(GeneratedUrl);

        private string _error = "";
        public string Error
        {
            get => _error;
            set
            {
                SetProperty(ref _error, value);
                OnPropertyChanged(nameof(HasError));
            }
        }


        public bool HasError => !string.IsNullOrEmpty(Error);

        private string _successMessage = "";
        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                SetProperty(ref _successMessage, value);
                OnPropertyChanged(nameof(HasSuccess));
            }
        }

        public bool HasSuccess => !string.IsNullOrEmpty(SuccessMessage);

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private ObservableCollection<TinyUrlEntry> _urls = new();
        public ObservableCollection<TinyUrlEntry> Urls
        {
            get => _urls;
            set => SetProperty(ref _urls, value);
        }

        // ─── Commands ─────────────────────────────────────────

        public AsyncRelayCommand GenerateCommand { get; }
        public AsyncRelayCommand LoadCommand { get; }
        public RelayCommand CopyCommand { get; }
        public AsyncRelayCommand DeleteCommand { get; }

        // ─── Constructor ──────────────────────────────────────

        public MainViewModel(UrlService svc)
        {
            _svc = svc;
            //_svc = new UrlService();

            GenerateCommand = new AsyncRelayCommand(
                _ => GenerateAsync(),
                _ => !string.IsNullOrEmpty(LongUrl));

            LoadCommand = new AsyncRelayCommand(
                _ => LoadUrlsAsync());

            CopyCommand = new RelayCommand(
                param => CopyToClipboard(param as string));

            DeleteCommand = new AsyncRelayCommand(
                param => DeleteAsync(param as string));

            // Load URLs on startup
            _ = LoadUrlsAsync();
        }

        // ─── Methods ──────────────────────────────────────────

        private string ValidateUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return ""; 

            var testUrl = url.Trim();

            if (!testUrl.StartsWith("http://") &&
                !testUrl.StartsWith("https://"))
                testUrl = "https://" + testUrl;

            if (!Uri.TryCreate(testUrl, UriKind.Absolute, out var uri))
                return "Invalid URL — please enter a valid web address";

            if (uri.Scheme != "http" && uri.Scheme != "https")
                return "Invalid URL — must start with http or https";

            if (string.IsNullOrEmpty(uri.Host) || uri.Host.Length < 3)
                return "Invalid URL — missing domain";

            if (!uri.Host.Contains('.'))
                return "Invalid URL — must include domain (e.g. google.com)";

            if (url.Contains(' '))
                return "URL cannot contain spaces";

            return "";
        }

        private async Task LoadUrlsAsync()
        {
            try
            {
                IsLoading = true;
                Error = "";
                var urls = await _svc.GetPublicAsync(Search);
                Urls = new ObservableCollection<TinyUrlEntry>(urls);
            }
            catch
            {
                Error = "Failed to load URLs. Is the API running?";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GenerateAsync()
        {
            UrlError = ValidateUrl(LongUrl);
            if (string.IsNullOrEmpty(LongUrl))
            {
                UrlError = "URL is required";
                return;
            }
            if (!string.IsNullOrEmpty(UrlError)) return;

            try
            {
                Error = "";
                var shortUrl = await _svc.AddAsync(LongUrl, IsPrivate);
                if (shortUrl != null)
                {
                    GeneratedUrl = shortUrl;
                    LongUrl = "";
                    UrlError = "";
                    await LoadUrlsAsync();
                }
            }
            catch
            {
                Error = "Failed to generate URL.";
            }
        }

        private async Task DeleteAsync(string? code)
        {
            if (string.IsNullOrEmpty(code)) return;
            try
            {
                await _svc.DeleteAsync(code);
                await LoadUrlsAsync();
            }
            catch
            {
                Error = "Failed to delete URL.";
            }
        }

        private void CopyToClipboard(string? url)
        {
            if (string.IsNullOrEmpty(url)) return;
            Clipboard.SetText(url);
            SuccessMessage = "✅ Copied!";
            Task.Delay(2000).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                    SuccessMessage = "");
            });
        }
    }
}