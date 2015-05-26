/* main page start */
$(document).ready(function () {
    main.init();
});
var main = {
    init: function () {
        //main.toolTip("#ma", "<b>Boston, MA</b>", "bottom left", "top left", 10, 3);
    },
    toolTip: function (id, text, my, at, x, y, except) {
        $(id).not(except).qtip({
            content: {
                text: text
            },
            position: {
                target: 'mouse',
                my: my,
                at: at,
                viewport: $(window),
                adjust: {
                    x: x, y: y, // Minor x/y adjustments
                    resize: true, // Reposition on resize by default
                    method: 'shift none'
                }
            },
            style: {
                classes: 'ui-tooltip-custom ui-tooltip-tip',
                tip: false,
                width: 150,
                height: 30
            },
            hide: {
                target: false, // Defaults to target element
                event: 'mouseleave', // Hide on mouse out by default
                inactive: true, // Hide when inactive
                leave: 'window' // Hide when we leave the window
            },
        });
    }
};
/* main page end */