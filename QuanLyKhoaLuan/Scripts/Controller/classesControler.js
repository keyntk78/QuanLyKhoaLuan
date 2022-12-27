var classesController = {
    init: function () {
        classesController.registerEvent();
    },

    registerEvent: function () {
        $('#pageSize').change(function () {
            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            var major_id = $('#select_major_id').val();
            var department_id = $('#select_department_id').val();


            classesController.loadData(null, pageSize, keywork, major_id, department_id);
        });


        $('body').on('click', '#btnSearch', function () {


            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            var major_id = $('#select_major_id').val();
            var department_id = $('#select_department_id').val();


            classesController.loadData(null, pageSize, keywork, major_id, department_id);
           
        });

        $('#select_major_id').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            var major_id = $('#select_major_id').val();

            var department_id = $('#select_department_id').val();


            classesController.loadData(null, pageSize, keywork, major_id, department_id);

        });

        $('#select_department_id').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            var major_id = $('#select_major_id').val();
            var department_id = $('#select_department_id').val();


            classesController.loadData(null, pageSize, keywork, major_id, department_id);

        });


        $('body').on('click', '.btnDelete', function (e) {


            var id = $(this).data('id');


            swal({
                title: "Bạn có chắc xóa!",
                text: "Bạn sẽ không thể khôi phục lại được!",
                type: "error",
                confirmButtonText: "Xóa",
                showCancelButton: true,
                cancelButtonText: "Đóng"
            }).then((result) => {
                if (result.value) {
                    classesController.Delete(id);
                }
            });
        });

        $('body').on('click', '.btnDetail', function (e) {
            var id = $(this).data('id');
            classesController.Detail(id);
        });

        pageSize = $('#pageSize').val();
        classesController.loadData(null, pageSize, "", "", "");
    },

    //page, pageSize, keywork, department_id
    loadData: function (page, pageSize, keywork, major_id, department_id) {
      
        $.ajax({
            url: '/Admin/Classes/GetClassData',
            type: 'GET',
            /*data: { page: page, pageSize: pageSize, keywork: keywork, department_id: department_id },*/
            data: { page: page, pageSize: pageSize, keywork: keywork, major_id: major_id, department_id: department_id },
            success: function (res) {
                if (res.TotalItem >= 0) {


                    var item = res.Data;
                    console.log(item);
                    var html = "";
                    for (let i = 0; i < res.Data.length; i++) {

                        html += "<tr>";
                        html += ` <td>${i + 1}</td>`;
                        html += ` <td>${item[i].classes.code}</td>`;
                        html += ` <td>${item[i].classes.name}</td>`;
                        html += ` <td>${item[i].classes.Major.name}</td>`;
                        html += ` <td>${item[i].classes.Major.Department.name}</td>`;
                        html += `<td>`;
                        html += `<button data-id="${item[i].classes.class_id}" class="btn btn-danger btnDelete" title="Xóa" ><i class="fa-solid fa-trash"></i></button> | `
                        html += `<a  href="/Admin/Classes/Edit/${item[i].classes.class_id}"  class="btn btn-success btnEdit" title="Cập nhật"><i class="fa-solid fa-pen-to-square"></i></a> | 
                                <button data-id="${item[i].classes.class_id}" class="btn btn-info btnDetail" data-toggle="modal" data-target="#exampleModal" title="Xem chi tiết" ><i class="fa-solid fa-eye"></i></button>
                                </td >`
                        html += `</tr>`;
                    }
                }
                $('#show_data').html(html);
                classesController.Pagination(res.CurrentPage, res.NumberPage, res.PageSize);
            }
        })
    },

    Pagination: function (currentPage, numberpage, pageSize) {
        if (numberpage > 0) {
            var str = `<nav aria-label="Page navigation example"><ul class="pagination">`;
            if (currentPage != 1) {
                str += ` <li class="page-item"><a class="page-link" onclick="classesController.NextPage(${currentPage - 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-left"></i></a></li>`;
            }
            for (let i = 1; i <= numberpage; i++) {
                if (currentPage === i) {
                    str += `<li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
                } else {

                    str += `<li class="page-item"><a class="page-link" onclick="classesController.NextPage(${i}, ${pageSize})" href="javascript:void(0);">${i}</a></li>`;
                }
            }

            if (currentPage != numberpage) {
                str += ` <li class="page-item"><a class="page-link" onclick="classesController.NextPage(${currentPage + 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-right"></i></a></li>`;
            }
            str += `</ul></nav>`;
            $('#pagination').html(str);
        }
    },

    NextPage: function (page, pageSize) {


        var keywork = $('#keywork').val();
        pageSize = $('#pageSize').val();
        var major_id = $('#select_major_id').val();
        var department_id = $('#select_department_id').val();
        classesController.loadData(page, pageSize, keywork, major_id, department_id);
    },

    Delete: function (id) {
        $.ajax({
            url: "/Admin/Classes/Delete",
            type: "POST",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    majorController.loadData(null, pageSize, "");
                    Swal.fire(
                        'Đã Xóa!',
                        'Bạn đã xóa lớp thành công',
                        'success'
                    )
                } else {
                    Swal.fire(
                        'Xóa thất bại!',
                        'Thất bại.',
                        'error'
                    )
                }
            }
        });
    },

    Detail: function (id) {
        $.ajax({
            url: "/Admin/Classes/Detail",
            type: "GET",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    $('#code').text(res.Data.code);
                    $('#name').text(res.Data.name);
                    $('#department').text(res.Data.Major.Department.name);
                    $('#major').text(res.Data.Major.name);
                    $('#decs').text(res.Data.description);
                } else {
                    alert("Có lỗi");
                }
            }
        });
    },

}

classesController.init();