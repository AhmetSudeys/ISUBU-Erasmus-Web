// SweetAlert ile silme onayı
document.addEventListener('DOMContentLoaded', function() {
    // Form submit'lerde SweetAlert kullan
    const deleteForms = document.querySelectorAll('form[action*="Delete"]');
    deleteForms.forEach(form => {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            Swal.fire({
                title: 'Emin misiniz?',
                text: "Bu işlem geri alınamaz!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Evet, Sil!',
                cancelButtonText: 'İptal'
            }).then((result) => {
                if (result.isConfirmed) {
                    form.submit();
                }
            });
        });
    });

    // Başarı mesajlarını SweetAlert ile göster
    const successMessage = document.querySelector('.alert-success');
    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: 'Başarılı!',
            text: successMessage.textContent.trim(),
            timer: 3000,
            showConfirmButton: false
        });
    }

    // Hata mesajlarını SweetAlert ile göster
    const errorMessage = document.querySelector('.alert-danger');
    if (errorMessage && !errorMessage.closest('form')) {
        Swal.fire({
            icon: 'error',
            title: 'Hata!',
            text: errorMessage.textContent.trim()
        });
    }
});

