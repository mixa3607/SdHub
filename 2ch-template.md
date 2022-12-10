---
title: Шаблон шапки
description: 
published: true
date: 2022-11-30T08:46:21.163Z
tags: 
editor: markdown
dateCreated: 2022-11-18T23:51:44.600Z
---

Если хотите внести какие то изменения то на странице есть кнопка `Edit on Github` жмите её и либо описывайте точно ишью что и куда воткнуть, либо сразу кидайте пулл реквест. Чекаю минимум раз в день.

```text
[b]NovelAI и WaifuDiffusion тред[/b]
[i]Генерируем тяночек![/i]
Прошлый тред: https://arhivach.ng/thread/ https://t.me/000000
----------------------------------
[b]Руководство с нуля[/b]
http://ai-art-wiki.com/wiki/Stable_Diffusion/Absolute_beginners_guide/ru
https://wiki.diffai.xyz/free-tg-bots - Бесплатные телеграм боты
----------------------------------
[b]УСТАНОВКА НА ПК[/b]
Для комфортной работы нужна видеокарта с как минимум 6Gb памяти.
http://ai-art-wiki.com/wiki/Stable_diffusion/install/ru
https://teletype.in/@stablediffusion/PC_install_SD
Оптимизация для слабых ПК. Обычно это приводит к потере детерминированности и качества.
https://github.com/AUTOMATIC1111/stable-diffusion-webui/wiki/Optimizations
https://rentry.org/voldy#-running-on-4gb-and-under-
Гайд для АМД под винду: https://travelneil.com/stable-diffusion-windows-amd.html
----------------------------------
[b]УСТАНОВКА В ОБЛАКЕ[/b]
Если у вас слабое железо, можно поднять нейронку на гугл коллабе. Google Colab - облако, выделяемое гуглом с мощными CPU и TPU. Можно юзать бесплатно несколько часов каждый день. Обойти ограничение можно, создав второй аккаунт.
http://ai-art-wiki.com/wiki/Stable_diffusion/cloud/ru
https://rentry.org/244wt - список колабов
----------------------------------
[i]Асука тест (проверка работоспособности модели)[/i]:
>masterpiece, best quality, asuka langley sitting cross legged on a chair
Negative prompt:
>lowres, bad anatomy, bad hands, text, error, missing fingers, extra digit, fewer digits, cropped, worst quality, low quality, normal quality, jpeg artifacts, signature, watermark, username, blurry, artist name

[b]Если у тебя чёрные квадраты вместо сгенерированной картинки, пропиши в параметрах webui-user.bat [i]set COMMANDLINE_ARGS=--no-half-vae[/i] (Для карт 16 линейки можно [i]--precision full --no-half[/i])
Если у тебя мыльные картинки при использовании Highres. fix поставь в настройках галочку Upscale latent space image when doing hires. fix[/b]
----------------------------------
[b]МОДЕЛИ И МИКСЫ:[/b] 
Модель определяет стиль генерируемых картинок. Популярные модели, используемые в треде:
- Аниме: AntlersMix, Berrymix
- Хентай: Anonmix
- Реализм: HassanBlend
Подробнее о моделях: https://rentry.co/ng5wh
200+ моделей и их хэши https://static.nas1.gl.arkprojects.space/stable-diff/
----------------------------------
[b]КАК УЗНАТЬ, КАК БЫЛА СГЕНЕРИРОВАНА КАРТИНКА?[/b]
Обычно данные о промптах, модели и прочем хранятся в ее метаданных. Но борда их удаляет, поэтому только спрашивать.
Картинки, сохраненные из интернета, можно проверить на вкладке "Метаданные PNG" или при помощи любого инструмента для чтения метаданных, например, identify.
----------------------------------
[b]У МЕНЯ НА ПИКЧАХ ГЛАЗА ВСРАТЫЕ, ФИОЛЕТОВЫЕ ПЯТНА, БЛЕДНЫЕ ЦВЕТА[/b]
Скачать VAE и включить в настройках.
Если лицо занимает малую часть пикчи, то либо увеличивать разрешение, либо вырезать ее и догенерировать в изо-в-изо.
----------------------------------
[b]АНОН ЗАПОСТИЛ ПИКЧУ ГДЕ ДЕД В КЕПКЕ С БАЯНОМ И ИИСУС СИДЯТ НА СЛОНЕ С ГОЛУБЫМИ ГЛАЗАМИ, А ВОКРУГ ЛЕТАЮТ АНГЕЛЫ. КАК ТАКОЕ СГЕНЕРИРОВАТЬ?[/b]
Большие детализированные пикчи обычно доделываются при помощи изо-в-изо и ручного дорисовывания.
----------------------------------
[b]ОБУЧЕНИЕ[/b]
Существующую модель можно обучить симулировать определенный стиль или рисовать конкретного персонажа или объект.
Если модель уже умеет рисовать что-то похожее: https://github.com/AUTOMATIC1111/stable-diffusion-webui/wiki/Textual-Inversion
Если она не умеет, или нужно делать это на нескольких моделях: https://rentry.org/hypernetwork4dumdums
Гайд по созданию своих моделей: https://github.com/nitrosocke/dreambooth-training-guide
Сборник гайдов и технических статей по теме: http://ai-art-wiki.com/wiki/Textual_inversion
----------------------------------
Репозитории: 
https://github.com/AUTOMATIC1111/stable-diffusion-webui - пользовательский интерфейс от AUTOMATIC1111
https://github.com/averad/OnnxDiffusersUI - интерфейс для владельцев AMD (если не получилось запустить автоматик)
----------------------------------
[b]ПОЛЕЗНЫЕ ССЫЛКИ[/b]
https://wiki.diffai.xyz/FAQ - [b]ЧаВо ДАННОГО ТРЕДА, ЕСЛИ ЕСТЬ ВОПРОС, ДЛЯ НАЧАЛА ЗАГЛЯНИ СЮДА[/b]
https://rentry.org/sdupdates2 - Вся информация о моделях, эмбедингах, промтах и т.д. (переодически обновляется)
https://wiki.diffai.xyz/training-info - [b]Обучение своих эмбедингов/моделей[/b]
https://wiki.diffai.xyz/other-info - Остальные ссылки (промпты, лица, одежда, позы, ракурсы и т.д.)
----------------------------------
Шаблон шапки для тредов: https://wiki.diffai.xyz/2ch-template
```
