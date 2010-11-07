/// <reference path="jquery.js" />

//add onto the jquery namespace
(function ($) {

    $(document).ready(function () {

        //let CSS know we have js.
        $('body').addClass('has-js');

        //hijax episode check boxes.
        $('#EpisodeIds').live("click", function () {
            var jThis = $(this);
            var watched = jThis.is(':checked');
            var jSeasonNumbers = jThis.parents('li:eq(1)').find('#SeasonNumbers');

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

                    //set line-thru
                    if (jSeasonNumbers.is(':checked')) {
                        jThis.parents('li:eq(1)').addClass('watched');
                    }
                    else {
                        jThis.parents('li:eq(1)').removeClass('watched');
                    }
                },
                error: handleError
            });

            //update season checkbox.
            if (jSeasonNumbers.parents('li').find('ul li input:not(:checked)').length == 0) {
                jSeasonNumbers.checkbox('check');
            }
            else {
                jSeasonNumbers.checkbox('uncheck');
            }

        });

        //hide non-ajax submit.
        $('#btnSaveWatched').hide();

        //add option to mark entire season watched.
        $('#episodes form > ul > li').each(function () {
            //add checkbox.
            var jThis = $(this);
            var season = jThis.find('.season').text().replace('Season', '').trim();
            //check if it should be checked (all episodes watched):
            var checked = jThis.find('input:not(:checked)').length == 0;
            var sChecked = '';
            if (checked) {
                sChecked = ' checked="checked"';
                //line-thru whole season.
                jThis.addClass('watched');
            }
            $(this).find('ul').before('<input type="checkbox" value="' + season + '" name="SeasonNumbers" id="SeasonNumbers"' + sChecked + ' />');

            //determine collapsible state.
            var collapseState = 'ui-icon-minus';
            if (checked) {
                collapseState = 'ui-icon-plus';
                //hide child list to start.
                $(this).find('ul').hide();
            }
            //add collapsible button.
            $(this).find('.season').prepend('<span class="collapsible ui-icon ' + collapseState + '"></span>');


        });

        $('#SeasonNumbers').live("click", function () {
            //check/uncheck all children.
            var jThis = $(this);
            var season = jThis.val();
            var seriesId = jThis.parents('#episodes').attr('data-seriesid');
            var watched = jThis.is(':checked');
            var jEpisodeInputs = jThis.parents('li').find('li input');
            if (watched) {
                jEpisodeInputs.checkbox('check');
            }
            else {
                jEpisodeInputs.checkbox('uncheck'); //jEpisodeInputs.checkbox().uncheck(); //.attr('checked', watched);
            }
            //make ajax call to update db.
            $.ajax({
                url: '/WatchSeason',
                type: 'POST',
                data: JSON.stringify({ SeriesId: seriesId, Season: season, Watched: watched }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function () {
                    if (watched) {
                        jEpisodeInputs.parents('li').addClass('watched');
                    }
                    else {
                        jEpisodeInputs.parents('li').removeClass('watched');
                    }
                },
                error: handleError
            });

        });

        //make season headers collapsible.
        $('.season').live("click", function () {
            var jThis = $(this);
            var jCollapsible = jThis.find('.collapsible');
            var jChildUL = jThis.parent().find('ul');
            if (jCollapsible.hasClass('ui-icon-minus')) {
                jChildUL.hide('blind');
                jCollapsible.removeClass('ui-icon-minus').addClass('ui-icon-plus');
            }
            else {
                jChildUL.show('blind');
                jCollapsible.removeClass('ui-icon-plus').addClass('ui-icon-minus');
            }
        });



        //style buttons and checkboxes
        $('.button').button();
        $('input[type=checkbox]').checkbox();

    });

    function handleError(xhr, err) {
        $.flashError(xhr.responseText);
    };

})(jQuery);