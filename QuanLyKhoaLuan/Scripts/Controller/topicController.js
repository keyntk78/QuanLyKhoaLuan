var topicController = {
    init: function () {
        topicController.registerEvent();
    },

    registerEvent: function () {

        $('#pageSize').change(function () {
            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            var department_id = $('#select_department_id').val();
            topicController.loadData(null, pageSize, keywork, department_id);
        });

        $('body').on('click', '#btnSearch', function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            var department_id = $('#select_department_id').val();
            topicController.loadData(null, pageSize, keywork, department_id);
        })


        $('#select_department_id').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            var department_id = $('#select_department_id').val();
            topicController.loadData(null, pageSize, keywork, department_id);

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
                    topicController.Delete(id);
                }
            });
        });

        $('body').on('click', '.btnDetail', function (e) {
            var id = $(this).data('id');
            topicController.Detail(id);
        });


        pageSize = $('#pageSize').val();
        topicController.loadData(null, pageSize, "", "");
    },

    //page, pageSize, keywork, department_id
    //page: page, pageSize: pageSize, keywork: keywork, department_id: department_id
    loadData: function (page, pageSize, keywork, department_id) {
        $.ajax({
            url: '/Admin/Topics/GetTopicData',
            type: 'GET',
            data: { page: page, pageSize: pageSize, keywork: keywork, department_id: department_id },
            success: function (res) {
                if (res.TotalItem >= 0) {
                    var item = res.Data;
                    var html = "";
                    for (let i = 0; i < res.Data.length; i++) {

                        html += "<tr>";
                        html += ` <td>${i + 1}</td>`;
                        html += ` <td>${item[i].topic.name}</td>`;
                        html += ` <td>${item[i].department.name}</td>`;
                        html += `<td>`;
                        html += `<button data-id="${item[i].topic.topic_id}" class="btn btn-danger btnDelete" title="Xóa" ><i class="fa-solid fa-trash"></i></button> | `
                        html += `<a  href="/Admin/Topics/Edit/${item[i].topic.topic_id}"  class="btn btn-success btnEdit" title="Cập nhật"><i class="fa-solid fa-pen-to-square"></i></a> | 
                                <button data-id="${item[i].topic.topic_id}" class="btn btn-info btnDetail" data-toggle="modal" data-target="#exampleModal" title="Xem chi tiết" ><i class="fa-solid fa-eye"></i></button>
                                </td >`
                        html += `</tr>`;
                    }
                }
                $('#show_data').html(html);
                topicController.Pagination(res.CurrentPage, res.NumberPage, res.PageSize);
            }
        })
    },
    Pagination: function (currentPage, numberpage, pageSize) {
        if (numberpage > 0) {
            var str = `<nav aria-label="Page navigation example"><ul class="pagination">`;
            if (currentPage != 1) {
                str += ` <li class="page-item"><a class="page-link" onclick="topicController.NextPage(${currentPage - 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-left"></i></a></li>`;
            }
            for (let i = 1; i <= numberpage; i++) {
                if (currentPage === i) {
                    str += `<li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
                } else {

                    str += `<li class="page-item"><a class="page-link" onclick="topicController.NextPage(${i}, ${pageSize})" href="javascript:void(0);">${i}</a></li>`;
                }
            }

            if (currentPage != numberpage) {
                str += ` <li class="page-item"><a class="page-link" onclick="topicController.NextPage(${currentPage + 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-right"></i></a></li>`;
            }
            str += `</ul></nav>`;
            $('#pagination').html(str);
        }
    },

    NextPage: function (page, pageSize) {
        var keywork = $('#keywork').val();
        pageSize = $('#pageSize').val();

        var department_id = $('#select_department_id').val();
        topicController.loadData(page, pageSize, keywork, department_id);
    },

    Delete: function (id) {
        $.ajax({
            url: "/Admin/Topics/Delete",
            type: "POST",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    topicController.loadData(null, pageSize, "", "");
                    Swal.fire(
                        'Đã Xóa!',
                        'Bạn đã xóa đề tài thành công',
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
            url: "/Admin/Topics/Detail",
            type: "GET",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    $('#name').text(res.Data.topic.name);
                    $('#department').text(res.Data.department.name);
                    $('#decs').text(res.Data.topic.description);
                } else {
                    alert("Có lỗi");
                }
            }
        });
    },

};

topicController.init();