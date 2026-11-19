document.addEventListener('DOMContentLoaded', function () {
    var nextBtn = document.getElementById('sd-next-btn');
    var backBtn = document.getElementById('sd-back-btn');
    var openBtn = document.getElementById('sd-cardtype-open');
    var cancelBtn = document.getElementById('sd-cardtype-cancel');
    var modal = document.getElementById('sd-cardtype-modal');

    if (nextBtn) nextBtn.addEventListener('click', function () {
        var required = ['Name', 'LastName', 'UserName', 'Street', 'City', 'State', 'ZipCode', 'Country'];
        for (var i = 0; i < required.length; i++) {
            var el = document.getElementById(required[i]);
            if (el && !el.value.trim()) {
                alert("Please fill all required fields before continuing.");
                el.focus();
                return;
            }
        }
        document.getElementById('sd-step1').style.display = 'none';
        document.getElementById('sd-step2').style.display = 'block';
        document.getElementById('sd-progress').classList.add('step2');
        document.getElementById('sd-step-badge').textContent = 'Step 2 / 2';
    });

    if (backBtn) backBtn.addEventListener('click', function () {
        document.getElementById('sd-step2').style.display = 'none';
        document.getElementById('sd-step1').style.display = 'block';
        document.getElementById('sd-progress').classList.remove('step2');
        document.getElementById('sd-step-badge').textContent = 'Step 1 / 2';
    });

    if (openBtn) openBtn.addEventListener('click', function () { modal.classList.add('open'); });
    if (cancelBtn) cancelBtn.addEventListener('click', function () { modal.classList.remove('open'); });

    var options = document.querySelectorAll('.sd-modal-option');
    options.forEach(function (opt) {
        opt.addEventListener('click', function () {
            document.getElementById('CardType').value = this.getAttribute('data-value');
            document.getElementById('sd-cardtype-label').textContent = this.getAttribute('data-label');
            modal.classList.remove('open');
        });
    });
});

document.addEventListener('DOMContentLoaded', function () {
    var toggle = document.getElementById('sd-eye-toggle');
    var pwd = document.getElementById('sd-pwd');

    if (toggle && pwd) {
        toggle.addEventListener('click', function () {
            pwd.type = pwd.type === 'password' ? 'text' : 'password';
        });
    }
});