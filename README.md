Wordy
=====

~intro todo~


Installation
--------------

1. (You need to have [.NET framework](http://www.microsoft.com/en-us/download/details.aspx?id=30653) installed on your computer)
2. Download [the release](https://github.com/Winterstark/Wordy/releases)
3. Extract
4. Run Wordy.exe


Usage
-------

### Adding words

To add new words click on the top button and enter a list of words in the textbox. After the definitions have been found you can edit them, and you probably should too.
I've found that usually a word will have several definitions with which I am already familiar with, so I filter those out and keep only those that I want to learn. Also, sometimes a word can have two definitions of basically the same meaning, only one of the definitions is a noun and the other is a verb (or another combination). In those instances I prefer to keep just one of the definitions.
Wordy has a built-in tutorial that will inform you about the process of adding words in detail.

### Learning words

To learn a word you will have to solve seven tests about it, each more difficult than the last:
1. Select the correct word (from 6 random words)
2. Select the correct word (from 6 words that sound very much the same)
3. Type the word
4. Fill out missing keywords in the word's definitions (one keyword per definition)
5. Fill out approx. half the missing keywords per definition
6. Fill out all of the keywords in the definitions

The release version of Wordy has several words set in various stages of learning, so you can see how these tests look. You can delete these words in Options.

Although this may seem overkill, the point of having so many tests (which become somewhat difficult near the end) is to help the user learn the word thoroughly. Many vocabulary services are much less demanding of their students, but their end result is also much less comprehensive and not as lasting. That is why Wordy is designed for either students that already have a good grasp of English, or native speakers that want to increase their vocabulary.

Note: after a word has been tested, it will not show up again until 24 hours pass.

### Remembering learned words

~~asdf~~

### Word of the day subscriptions

~~asdf~~


Alternate way of adding words
-------------------------------

If you use a program called [Launchy](http://www.launchy.net/) or use command prompt regulary, you might be interested in adding words to Wordy via a VB script. This way is potentially faster because you just have to activate Launchy and type: wordy ► [your word]. The "►" character stands for the Tab key.

To setup Launchy open its options and select Runner under the Plugins tab. Add a new line and configure it as in the following picture:

![Launchy configuration](http://i.imgur.com/n78kYeB.png)

Of course, you need to change the path to the "add words.vbs" script, which you can find in the Wordy directory.


APIs used & other credits
----------------------------

* Word definitions & examples: [Wordnik API](http://developer.wordnik.com/)
* Word visuals: [Flickr API](http://www.flickr.com/services/api/)
* Word rhymes: [RhymeBrain](http://rhymebrain.com/)
* Book icon by ~XTUX345 @ [deviantART](http://xtux345.deviantart.com/art/Elements-of-Harmony-Dictionary-Icon-280443607?q=boost%3Apopular%20dictionary%20icon&qo=9)
