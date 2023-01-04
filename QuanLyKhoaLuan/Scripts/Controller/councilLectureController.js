var councilLectureController = {
    init: function () {
        councilLectureController.registerEvent();
    },
    registerEvent: function () {
        $('#pageSize').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            councilLectureController.loadData(null, pageSize, keywork);
        });

        $('body').on('click', '#btnSearch', function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            councilLectureController.loadData(null, pageSize, keywork);

        });

        var pageSize = $('#pageSize').val();
        councilLectureController.loadData(null, pageSize, "");
    },

    //page, pageSize, keywork, shool_year_id
    loadData: function (page, pageSize, keywork) {
        $.ajax({
            url: '/Lecture/CouncilLecture/GetCouncilData',
            type: 'GET',
            //page: page, pageSize: pageSize, keywork: keywork, shool_year_id: shool_year_id
            data: { page: page, pageSize: pageSize, keywork: keywork },

            success: function (res) {
                if (res.TotalItem >= 0) {

                    var item = res.Data;
                    var html = "";
                    for (let i = 0; i < res.Data.length; i++) {
                        html += "<tr>";
                        html += ` <td>${i + 1}</td>`;
                        html += ` <td>${item[i].council.code}</td>`;
                        html += ` <td>${item[i].council.name}</td>`;
                        html += ` <td>${item[i].shool_year.name}</td>`;
                        if (item[i].council.is_block) {
                            html += `<td align="center"><span class="badge badge-primary"><i class="fa-regular fa-lock-open"></i></span></td>`;
                        } else {
                            html += `<td align="center"><span  class="badge badge-danger"><i class="fa-regular fa-lock"></i></span></td>`;
                        }
                        html += `<td>`;
                        html += `<a  href="CouncilLecture/ListThesis/${item[i].council.council_id}"  class="btn btn-info btnEdit" title="Danh sách khóa luận"><i class="fa-solid fa-solid fa-eye"></i></a>
                                </td >`
                        html += `</tr>`;
                    }
                }
                $('#show_data').html(html);
                councilLectureController.Pagination(res.CurrentPage, res.NumberPage, res.PageSize);
            }
        })
    },

    Pagination: function (currentPage, numberpage, pageSize) {
        if (numberpage > 0) {
            var str = `<nav aria-label="Page navigation example"><ul class="pagination">`;
            if (currentPage != 1) {
                str += ` <li class="page-item"><a class="page-link" onclick="councilLectureController.NextPage(${currentPage - 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-left"></i></a></li>`;
            }
            for (let i = 1; i <= numberpage; i++) {
                if (currentPage === i) {
                    str += `<li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
                } else {

                    str += `<li class="page-item"><a class="page-link" onclick="councilLectureController.NextPage(${i}, ${pageSize})" href="javascript:void(0);">${i}</a></li>`;
                }
            }

            if (currentPage != numberpage) {
                str += ` <li class="page-item"><a class="page-link" onclick="councilLectureController.NextPage(${currentPage + 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-right"></i></a></li>`;
            }
            str += `</ul></nav>`;
            $('#pagination').html(str);
        }
    },

    NextPage: function (page, pageSize) {


        var keywork = $('#keywork').val();
        pageSize = $('#pageSize').val();

        councilLectureController.loadData(page, pageSize, keywork);
    },
}

councilLectureController.init();
