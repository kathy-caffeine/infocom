# Тестовое задание

## Формат используемых данных:

### Номер машины: 
`XdddXX`, где `X` - заглавная латинская буква, `d` - цифра
### Веса
Число, до 3 знаков после запятой. В качестве разделителя целой и дробной части может использоваться как `.`, так и `,`, или ничего:
* `1.0`
* `1,0`
* `1`
### Даты
`yyyy-MM-dd`

## БД

Использовала SQL Express, запрос для создания базы:

```
create table dbo.data_records (
    car_id varchar(6) not null,
    gross_weight numeric(18, 3) not null,
    tare_weight numeric(18, 3) not null,
    net_weight numeric(18, 3) not null,
	gross_date date not null,
	tare_date date not null
);
```
