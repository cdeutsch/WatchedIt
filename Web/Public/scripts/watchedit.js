/// <reference path="jquery.js" />

//add onto the jquery namespace
(function ($) {

    $(document).ready(function () {
        $('.button').button();
        
        //hijax episode check boxes.
        $('#EpisodeIds').live("click", function () {
            var jThis = $(this);
            var watched = jThis.is(':checked');

            //make ajax call to update db.
            $.ajax({
                url: '/WatchEpisode',
                type: 'POST',
                data: JSON.stringify({ EpisodeId: jThis.val(), Watched: watched }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function () {
                    if (watched) {
                        jThis.parents('li:first').addClass('watched');
                    }
                    else {
                        jThis.parents('li:first').removeClass('watched');
                    }
                },
                error: handleError
            });

            //update season checkbox.
            var jSeasonNumbers = jThis.parent().parent().parent().find('#SeasonNumbers');
            jSeasonNumbers.attr('checked', jSeasonNumbers.parent().find('ul li input:not(:checked)').length == 0);
        });

        //hide non-ajax submit.
        $('#btnSaveWatched').hide();

        //add option to mark entire season watched.
        $('#episodes form > ul > li').each(function () {
            //add checkbox.
            var jThis = $(this);
            var season = jThis.clone().find("*").remove().end().text().replace('Season', '').trim();
            //check if it should be checked (all episodes watched):
            var checked = jThis.find('input:not(:checked)').length == 0;
            var sChecked = '';
            if (checked) {
                sChecked = ' checked="checked"';
            }
            $(this).find('ul').before('<input type="checkbox" value="' + season + '" name="SeasonNumbers" id="SeasonNumbers"' + sChecked + ' />');
        });

        $('input[type=checkbox]').checkbox();

        $('#SeasonNumbers').live("click", function () {
            //check/uncheck all children.
            var jThis = $(this);
            var season = jThis.val();
            var seriesId = jThis.parents('#episodes').attr('data-seriesid');
            var watched = jThis.is(':checked');
            var jEpisodeInputs = jThis.parent().find('li > input');
            jEpisodeInputs.attr('checked', watched);
            //make ajax call to update db.
            $.ajax({
                url: '/WatchSeason',
                type: 'POST',
                data: JSON.stringify({ SeriesId: seriesId, Season: season, Watched: watched }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function () {
                    if (watched) {
                        jEpisodeInputs.parents('li:first').addClass('watched');
                    }
                    else {
                        jEpisodeInputs.parents('li:first').removeClass('watched');
                    }
                },
                error: handleError
            });

        });
    });


    function updateSeasonChecked(target) {

    }

    function handleError(xhr, err) {
        $.flashError(xhr.responseText);
    };

})(jQuery);