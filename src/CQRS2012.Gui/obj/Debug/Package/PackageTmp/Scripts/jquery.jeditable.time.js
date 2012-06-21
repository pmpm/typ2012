/*
 * Timepicker for Jeditable
 *
 * Copyright (c) 2008-2009 Mika Tuupola
 *
 * Licensed under the MIT license:
 *   http://www.opensource.org/licenses/mit-license.php
 *
 * Project home:
 *   http://www.appelsiini.net/projects/jeditable
 *
 * Revision: $Id$
 *
 */

$.editable.addInputType('time', {
    /* Create input element. */
    element: function (settings, original) {
        /* Create and pulldowns for hours and minutes. Append them to */
        /* form which is accessible as variable this.                 */

        var hourselect = $('<input id="homeGoal" class="home-goals"  type="text" />').val("0");
        var minselect = $('<input id="guestGoal" class="guest-goals"  type="text" />').val(0);

        /*for (var hour=0; hour <= 99; hour++) {           
        var option = $('<option />').val(hour).append(hour);
        hourselect.append(option);
        }*/
        $(this).append(hourselect);
        $(this).append("<span style='font-size:12px'>:</span>");

        /*for (var min=0; min <= 99; min++) {          
        var option = $('<option />').val(min).append(min);
        minselect.append(option);
        }*/
        $(this).append(minselect);

        /* Last create an hidden input. This is returned to plugin. It will */
        /* later hold the actual value which will be submitted to server.   */
        var hidden = $('<input type="hidden" />');
        $(this).append(hidden);



        return (hidden);
    },
    /* Set content / value of previously created input element. */
    content: function (string, settings, original) {

        var splitedString = string.split(':');
        var splitedSomee = splitedString[1].split('<');      

        var goal1 = (splitedString[0] == "?") ? "" : splitedString[0];
        var goal2 = (splitedString[1] == "?") ? "" : splitedSomee[0];

        $('#homeGoal', this).val(goal1);
        $('#guestGoal', this).val(goal2);

    },
    /* Call before submit hook. */
    submit: function (settings, original) {
        /* Take values from hour and minute pulldowns. Create string such as    */
        /* 13:45 from them. Set value of the hidden input field to this string. */
        var value = $('#homeGoal').val() + ':' + $('#guestGoal').val();
        $('input', this).val(value);
    }
});
