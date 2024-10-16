
# Vehicles Control

ORM olarak Dapper ve Entity Framework Core kullanarak temel Crud işlemlerinin yapıldığı template bir projedir. Proje içerisinde Hangfire kullanarak background işemlerinin yapıldığı temel oluşturmaya çalıştım. Ek olarak memory cache konusuna da değindim.

Repository kısmında Generic Repository kullandım. Request ve Response kısımlarının eşlenmesi kısmında ise de AutoMapper kullandım.

Message queue kısmında ise de RabbitMQ kullanarak farklı apilerle veri haberleşmesini 
sağladım.

Authentication tarafında ise de JWT kullandım.

Test kısmında ise de FakeItEasy kullanarak api ve service methodlarımın testlerini yazdım.


# UI

UI kısmında Blazor kullandım ve MudBlazor kütüphanesini kullanarak aşağıdaki kısımları geliştirdim.

## Login Page

Login tarafında giriş bilgileri ile beraber dönen tokenı localstorage'a yazarak api isteklerinde kullandım.

![login](https://i.postimg.cc/3J8pMyJ0/Screenshot-2024-10-16-164139.png)

## User Page

![usr](https://i.postimg.cc/7ZTZjD7T/usr.png)

## Cars Page

![cars](https://i.postimg.cc/x8ydxPbp/cars.png)

## Car Detail Update Or Delete

![carsUpdateOrDelete](https://i.postimg.cc/9QPBP3My/carupdate.png)

## Imdb API
İmdb apisini kullanarak imdb sayfasında arama yaptığımız sayfa.

![imdb](https://i.postimg.cc/vDFW9XwP/imdb.png)

## Gemini AI
Gemini AI entegrasyonu ile ai chat sayfası.

![ai](https://i.postimg.cc/wvnWfXdN/ai.png)
