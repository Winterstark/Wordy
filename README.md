﻿Wordy
=====

Wordy is a Windows program that helps users improve their English vocabulary.

What sets it apart from other vocabulary-learning software?

- There are no predefined tests and quizzes that you can take. Instead, you can add any word you want to learn and Wordy will generate different types of questions and tests for that word.
- At first, these tests are simple (e.g. choose the correct word), but later on they involve filling out multiple definitions, which is what makes Wordy more suited for advanced students.
- Besides definitions words can have images associated with them to help with the learning process.
- After a word has been learned it becomes available in the long-term memory tests. There it appears much less frequently but using a different set of question types.
- Wordy also supports word of the day subscriptions, which are an easy way to discover new words.


Installation
--------------

1. (You need to have [.NET framework](http://www.microsoft.com/en-us/download/details.aspx?id=30653) installed on your computer)
2. Download [the release](https://github.com/Winterstark/Wordy/releases)
3. Extract
4. Run Wordy.exe


Usage
-------

### Adding words

To add new words enter a list of words in the leftmost textbox. After the definitions have been found you can edit them; words usually have multiple definitions and some of them are probably not pertinent to your inquiry. You should delete any irrelevant definitions and also superfluous wording - your future self will be grateful.

![Screenshot: adding words](http://i.imgur.com/KQrrqgf.png)

Any found visuals will be displayed in a row of small thumbnails. Mouse over one of the thumbnails to view the visual in full size. Left-click on it to open its Flickr page; right-click on it to remove it. After you delete some of them you can search for more visuals.

Wordy also has a built-in tutorial that will inform you about the process of adding words in more detail.

### Learning words

To learn a word you will have to solve six increasingly difficult tests about it:

1. Select the correct word (from 6 random words)
2. Select the correct word (from 6 words that sound very similar)
3. Type out the word
4. Fill out missing keywords in the word's definitions (one keyword per definition)
5. Fill out approx. half the missing keywords per definition
6. Fill out all of the keywords in the definitions

![Screenshot: test type 1](http://i.imgur.com/a4HDE22.png)

![Screenshot: test type 2](http://i.imgur.com/gTerbZj.png)

![Screenshot: test type 3](http://i.imgur.com/93S1w45.png) ![Screenshot: test type 3 with answers revealed](http://i.imgur.com/VMYhBxg.png)

Note that in the last screenshot the user correctly filled out only one of the definitions. This means the test is partially completed: the user will still have to repeat that test, but only for the failed definition.

Although this may seem like overkill, the point of having so many tests (which become somewhat difficult near the end) is to help the user learn the word thoroughly. Many vocabulary services are much less demanding of their students, but their end result is also much less comprehensive and not as lasting. That is why Wordy is designed for either students that already have a good grasp of English, or native speakers that want to improve their vocabulary.

After a word has been tested, the following test will become available after 24 hours.

The release version of Wordy has several example words set in various stages of learning so you can see how these tests look. You can delete these words in the Options menu.

### Remembering learned words

To ensure that the words you learn remain in your long-term memory you should keep testing them with the "Test Recall of Learned Words" option. Learned words will have different types of tests, and they will be picked randomly:
* Type out the word
* Fill in the blanks in the definitions
* Select only the correct definitions
* Complete the word based on its synonyms
* Enter the word that fits the example sentence
* Unscramble the letters of the word
* Recognize the word by its visuals

![Screenshot: test type 1](http://i.imgur.com/YUNCp7g.png)

![Screenshot: test type 2](http://i.imgur.com/fltHjpN.png)

![Screenshot: test type 3](http://i.imgur.com/Bnx2Gv5.png)

![Screenshot: test type 4](http://i.imgur.com/4Scmch1.png)

If you fail a test then the word becomes "unlearned" again; answer correctly and the word will be available for testing again in a week. After each successful test the length of time before the next test increases by a week.

When testing learned words, some minor mistakes are ignored. If you forget to use a diacritic mark or a hyphen, your answer will still be correct. Synonyms of the requested word will also be accepted.

Visuals may also be used for testing purposes.

![Screenshot: test type 5](http://i.imgur.com/QKCxEBD.png) ![Screenshot: test type 5 with displayed visuals](http://i.imgur.com/Hbf9ZQt.png)

### Word of the day subscriptions

Wordy comes with several WotD subscriptions; you can disable or delete them in the Options, as well as add new ones. To add a new subscription you need a valid RSS address.

Once per day Wordy will check the subscriptions and if there are any updates a new button will appear in the main menu. Clicking on that button will open the Add Words window where the new WotDs will be shown for the user to edit the definitions and either reject or accept them.

### Options

In the Options you can view a list of all the words you've added, edit their definitions and synonyms, and delete them. You can also edit word of the day subscriptions, as well as modify other general options.

![Screenshot: options](http://i.imgur.com/Dn5UhuF.png)


Alternate way of adding words
-------------------------------

If you use a program called [Launchy](http://www.launchy.net/) or use command prompt regulary, you might be interested in adding words to Wordy via a VB script. This way is potentially faster because you just have to activate Launchy and type: wordy ► [your word]. The "►" character stands for the Tab key.

To setup this script you need to perform the following:

* In Launchy, open its options and select Runner under the Plugins tab. Add a new line and configure it as in the following picture:

![Launchy configuration](http://i.imgur.com/n78kYeB.png)

Of course, you need to change the path to the "add words.vbs" script, which you can find in the Wordy directory.

* Edit "add words.vbs" and set strTxtFile to be equal to the path of an empty text file that will store the added words.

* Open Wordy's options and set the path to that same text file in this textbox:

![Screenshot: Wordy configuration](http://i.imgur.com/nbJxR1P.png)


APIs used & other credits
----------------------------

* Word definitions & examples: [Wordnik API](http://developer.wordnik.com/)
* Word visuals: [Flickr API](http://www.flickr.com/services/api/)
* Word rhymes: [RhymeBrain](http://rhymebrain.com/)
* Book icon by ~XTUX345 @ [deviantART](http://xtux345.deviantart.com/art/Elements-of-Harmony-Dictionary-Icon-280443607?q=boost%3Apopular%20dictionary%20icon&qo=9)
* Checkmark graphics: [psdGraphics](http://www.psdgraphics.com/psd-icons/psd-check-and-cross-icons/)
