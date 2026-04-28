document.addEventListener('DOMContentLoaded', function () {
    const tabs = document.querySelectorAll('#documentSteps .nav-link');
    const panels = document.querySelectorAll('.doc-panel');

    tabs.forEach(tab => {
        tab.addEventListener('click', function (e) {
            e.preventDefault();

            const target = e.target.getAttribute('data-target');

            // Hide all panels
            panels.forEach(panel => {
                panel.classList.add('d-none');
            });

            // Show the selected panel
            const targetPanel = document.getElementById(`panel-${target}`);
            if (targetPanel) {
                targetPanel.classList.remove('d-none');
            }

            // Set active class on the clicked tab
            tabs.forEach(tab => {
                tab.classList.remove('active');
            });
            tab.classList.add('active');
        });
    });
});
