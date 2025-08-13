using MauiApp1.ViewModels;
using MauiApp1.Models;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
        
        // 监听消息变化，自动滚动到底部
        if (BindingContext is MainViewModel viewModel)
        {
            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.CurrentSession))
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // 减少延迟，让滚动更及时
                        Task.Delay(50).ContinueWith(_ => 
                        {
                            MainThread.BeginInvokeOnMainThread(() => ScrollToBottom());
                        });
                    });
                }
            };
        }
    }

    private void ScrollToBottom()
    {
        if (ChatCollectionView.ItemsSource is System.Collections.IEnumerable items)
        {
            var count = items.Cast<object>().Count();
            if (count > 0)
            {
                ChatCollectionView.ScrollTo(count - 1, animate: true);
            }
        }
    }
}
