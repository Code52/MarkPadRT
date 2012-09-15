#Win8 Jekyll "app site" templates

Many apps are relatively simple, and don't need a fully blown websites to accompany them - usually a simple, static, single page is suffice.

###Setup
[GitHub pages](http://pages.github.com/) for hosting and [Jekyll](http://github.com/mojombo/jekyll/) for templating/static site generation is a perfect fit for this scenario, but keep an eye out on [Code52](http://code52.org/) for alternative Jekyll generators.

1. Create a GitHub repo
2. Enable GH-Pages
3. Run some git commands.

       	git clone git://github.com/Code52/HAMMER.YELLS.git
       	cd Win8SiteTemplate
       	git remote add MySite <yourGitHubUrl>
       	git symbolic-ref HEAD refs/heads/gh-pages
    
4.  You'll then need to customise your site, open up `_config.yml` and you'll see a bunch of settings like 'your app name', 'accent' (that is the "blue" colour in the example below), your AppId etc. 

5. The easiest way to 'fit' it into the gap of the tablet is to use a distort tool to drag into each of the corners. The gap is about 450x310 pixels.

5. Deploy your site with more git!

    	git commit -m "init"
    	git push MySite gh-pages

6. Within 10 minutes (it's only "that slow" the first time), your site will be running at http://YOURNAME.github.com/REPONAME/. You can even use your own domain, but look at the [GitHub Pages documentation for that](http://pages.github.com/)

###Todo
* Dynamic number of screenshots (set in config)
* More social integration 