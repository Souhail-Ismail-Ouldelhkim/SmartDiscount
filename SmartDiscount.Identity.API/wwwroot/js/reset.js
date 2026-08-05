document.addEventListener('DOMContentLoaded', function () {
    var boxes = document.querySelectorAll('.sd-code-box');
    var hidden = document.getElementById('sd-code-hidden');

    if (boxes.length === 0) return;

    function updateHidden() {
        var code = '';
        boxes.forEach(function (b) { code += b.value; });
        if (hidden) hidden.value = code;
    }

    boxes.forEach(function (box, index) {
        box.addEventListener('input', function () {
            this.value = this.value.replace(/[^0-9]/g, '');  
            if (this.value && index < boxes.length - 1) {
                boxes[index + 1].focus();
            }
            updateHidden();
        });

        box.addEventListener('keydown', function (e) {
            if (e.key === 'Backspace' && !this.value && index > 0) {
                boxes[index - 1].focus();
            }
        });

        box.addEventListener('paste', function (e) {
            e.preventDefault();
            var paste = (e.clipboardData || window.clipboardData).getData('text').replace(/[^0-9]/g, '');
            for (var i = 0; i < boxes.length && i < paste.length; i++) {
                boxes[i].value = paste[i];
            }
            updateHidden();
            if (paste.length >= boxes.length) boxes[boxes.length - 1].focus();
        });
    });
});