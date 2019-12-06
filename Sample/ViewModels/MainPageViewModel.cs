using System;
using Prism.Mvvm;
using Reactive.Bindings;
using Prism.Navigation;
using System.Collections.ObjectModel;
using Sample.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Sample.ViewModels
{
    public class MainPageViewModel:BindableBase,INavigatedAware
    {
        public ObservableCollection<WebBook> Books { get; set; }
        public ReactivePropertySlim<int> Position { get; } = new ReactivePropertySlim<int>(0);
        public ReactivePropertySlim<WebBook> CurrentItem { get; } = new ReactivePropertySlim<WebBook>();
        public AsyncReactiveCommand MoreCommand { get; } = new AsyncReactiveCommand();
        public ReactiveCommand PrevCommand { get; } = new ReactiveCommand();
        public ReactiveCommand NextCommand { get; } = new ReactiveCommand();

        int _nextPage = 0;
        IWebApi _webApi;
        public MainPageViewModel(IWebApi webApi)
        {
            _webApi = webApi;

            MoreCommand.Subscribe(async _ =>
            {
                foreach(var book in await GetData())
                {
                    Books.Add(book);
                }
            });

            PrevCommand.Subscribe(_ =>
            {
                if(Position.Value > 0)
                {
                    Position.Value--;
                }
            });

            NextCommand.Subscribe(_ =>
            {
                if (Position.Value < Books.Count - 1)
                {
                    Position.Value++;
                }
            });
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            _ = InitializeData();
        }

        public async Task InitializeData()
        {
            Books = new ObservableCollection<WebBook>(await GetData());
            RaisePropertyChanged(nameof(Books));
            Position.Value = 1;
        }

        public async Task<List<WebBook>> GetData()
        {
            var books = await _webApi.GetByKeyword("Xamarin", 6, _nextPage);
            if(books.Count() < 6)
            {
                _nextPage = 0;
            }
            else
            {
                _nextPage += 6;
            }

            return books.ToList();
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }


    }
}
